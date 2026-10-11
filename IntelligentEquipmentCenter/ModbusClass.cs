using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyModbus;
using Sunny.UI;
using static EasyModbus.ModbusServer;

namespace IntelligentEquipmentCenter
{
    /// <summary>
    /// PLC 32位数据(浮点/32位整型)的字节顺序。
    /// 以IEEE位模式0x3F800000(浮点数1.0)为例，四种排列对应的寄存器内容如下：
    /// </summary>
    public enum PlcByteOrder
    {
        ABCD = 0,   // 高字在前，字内高字节在前：寄存器[0x3F80, 0x0000]（标准大端，汇川多数默认）
        BADC = 1,   // 高字在前，字内字节交换：寄存器[0x803F, 0x0000]
        CDAB = 2,   // 低字在前，字内高字节在前：寄存器[0x0000, 0x3F80]
        DCBA = 3,   // 低字在前，字内字节交换：寄存器[0x0000, 0x803F]（完全小端）
    }

    /// <summary>
    /// Modbus操作类，实现对不同的数据读取写入采用线程的方式
    /// </summary>
    static class ModbusClass
    {
        //private static string name;

        private static readonly object obj = new object();//互锁

        private static readonly ModbusClient modbus = new ModbusClient();//modbus客户端

        static ModbusClass()
        {
            //连接超时：PLC未上电/网线不通时，避免Connect()长时间阻塞(默认可能拖很久)
            modbus.ConnectionTimeout = 1000;
        }

        /// <summary>
        /// PLC字节序，默认ABCD(标准大端)。如需匹配不同PLC，在程序启动时设置此项即可，
        /// 所有字符串/浮点数读写都按此字节序统一处理。
        /// </summary>
        public static PlcByteOrder byteOrder = PlcByteOrder.CDAB;

        private static ushort Swap16(ushort v) => (ushort)((v << 8) | (v >> 8));

        /// <summary>把两个寄存器(reg0=低地址, reg1=高地址)按当前byteOrder组合成IEEE-754位模式</summary>
        public static int CombineToIntBits(int reg0, int reg1)
        {
            ushort w0 = (ushort)(reg0 & 0xFFFF);
            ushort w1 = (ushort)(reg1 & 0xFFFF);
            switch (byteOrder)
            {
                case PlcByteOrder.ABCD: return (w0 << 16) | w1;
                case PlcByteOrder.BADC: return (Swap16(w0) << 16) | Swap16(w1);
                case PlcByteOrder.CDAB: return (w1 << 16) | w0;
                case PlcByteOrder.DCBA: return (Swap16(w1) << 16) | Swap16(w0);
                default: return (w0 << 16) | w1;
            }
        }

        /// <summary>把两个寄存器组合成浮点数</summary>
        public static float CombineToFloat(int reg0, int reg1)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(CombineToIntBits(reg0, reg1)), 0);
        }

        /// <summary>把浮点数按当前byteOrder拆成两个寄存器(reg0=低地址, reg1=高地址)</summary>
        public static void SplitFromFloat(float value, out int reg0, out int reg1)
        {
            int bits = BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
            ushort high = (ushort)((bits >> 16) & 0xFFFF);
            ushort low = (ushort)(bits & 0xFFFF);
            switch (byteOrder)
            {
                case PlcByteOrder.ABCD: reg0 = high; reg1 = low; break;
                case PlcByteOrder.BADC: reg0 = Swap16(high); reg1 = Swap16(low); break;
                case PlcByteOrder.CDAB: reg0 = low; reg1 = high; break;
                case PlcByteOrder.DCBA: reg0 = Swap16(low); reg1 = Swap16(high); break;
                default: reg0 = high; reg1 = low; break;
            }
        }

        //多线程同时读写，用volatile保证一个线程改了连接状态其它线程能立即看到(否则可能一直读到旧值)
        public static volatile bool connected = false;

        public static int address = 10000;
        public static bool isConnected()
        {
            //return modbus.Connected;
            return connected;
        }

        /// <summary>
        /// 连接Modbus服务端
        /// </summary>
        /// <param name="ip">服务端IP地址</param>
        /// <param name="port">服务端端口号</param>
        /// <returns>连接状态</returns>
        public static bool ConnectToServer(string ip,int port)
        {
            //多个轮询线程都会各自触发重连，必须加锁，否则可能同时对同一个socket发起Connect
            lock (obj)
            {
                if(!modbus.Connected)
                {
                    try
                    {
                        modbus.Connect(ip, port);
                        connected = true;
                        return connected;//modbus.Connected;
                    }
                    catch ( Exception ex)
                    {
                        //原来只Console.WriteLine，正式exe无控制台时看不到；改为写入日志文件
                        Logger.push(new Log(Logger.LogType.ERROR, "Modbus", "PLC连接失败：" + ip + ":" + port + " " + ex.Message));
                        connected = false;
                        return false;
                    }
                }
                else
                {
                    //return modbus.Connected;
                    return connected;
                }
            }
        }
        /// <summary>
        /// 断开modbus服务器
        /// </summary>
        /// <returns>断开情况</returns>
        public static bool DisconnectForServer()
        {
            //整个"判断+断开"必须原子，否则断开的同时其它线程可能正在读同一个socket
            lock (obj)
            {
                if(modbus.Connected)
                {
                    modbus.Disconnect();
                }
                return !modbus.Connected;
            }
        }

        //public bool[] ReadCoils(int startingAddress, int quantity)
        //public bool[] ReadDiscreteInputs(int startingAddress, int quantity)
        //public int[] ReadHoldingRegisters(int startingAddress, int quantity)
        //public int[] ReadInputRegisters(int startingAddress, int quantity)
        //public int[] ReadWriteMultipleRegisters(int startingAddressRead, int quantityRead, int startingAddressWrite, int[] values)

        //public void WriteMultipleCoils(int startingAddress, bool[] values)
        //public void WriteMultipleRegisters(int startingAddress, int[] values)
        //public void WriteSingleCoil(int startingAddress, bool value)
        //public void WriteSingleRegister(int startingAddress, int value)

        //X0-X1777（8进制），长度1024，地址0xF800-0xFBFF(63488-64511)，功能码：读线圈(02)
        //一次最多可以读取2000个线圈，汇川1024个小于2000

        //Y0-Y1777（8进制），长度1024，地址0xFC00-0xFFFF(64512-65535)，功能码：读线圈(01) 
        //一次最多可以读取2000个线圈，汇川1024个小于2000

        //M0-M8000，长度1024，地址0x0000-0x1F3F(0-7999)，功能码：读线圈(01)
        //一次最多可以读取2000个线圈，汇川8000个大于2000

        //M0-X7999，长度8000，地址0x0000-0x1F3F(0-7999)，功能码：写单线圈(05) 
        //Y0-Y1777（8进制），长度1024，地址0xFC00-0xFFFF(64512-65535)，功能码：写单线圈(05) 
        //一般只写M，不建议写Y,防止风险。

        //M0-X7999，长度8000，地址0x0000-0x1F3F(0-7999)，功能码：写多线圈(15) 
        //Y0-Y1777（8进制），长度1024，地址0xFC00-0xFFFF(64512-65535)，功能码：写多线圈(15) 
        //最多只能连续写16个

        //D0-D7999，长度8000，地址0x0000-0x1F3F(0-7999)，功能码：读寄存器(03) 
        //数据格式 =1为16位 =2为32位,均为有符号
        //注:最好单个读取或者多个连续读取,多个间断读取（如0-10 20-30 100-120）先调用地址号大的，从大到小依次调用
        //连续读取数量不能超过125个

        /// <summary>
        /// 读取线圈的值
        /// </summary>
        /// <param name="startingAddress">开始地址</param>
        /// <param name="quantity">读取长度</param>
        /// <returns>线圈的值数组</returns>
        public static bool[] ReadCoils(int startingAddress, int quantity)
        {
            bool[] coils = new bool[quantity];
            lock (obj)
            {
                //双保险：应用层connected标志优先，未连接秒回，避免界面线程阻塞在socket读上
                if (!modbus.Connected || !connected)
                    return coils;
                try
                {
                    coils = modbus.ReadCoils(startingAddress+ address, quantity);
                }
                catch (Exception)
                {
                    //PLC掉线/网线松动时读抛异常：置为断开让主轮询线程统一重连，返回默认值避免异常上抛崩溃UI线程
                    connected = false;
                }
            }
            return coils;
        }
        /// <summary>
        /// 读取输入
        /// </summary>
        /// <param name="startingAddress"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public static bool[] ReadDiscreteInputs(int startingAddress, int quantity)
        {
            bool[] inputs = new bool[quantity];
            lock (obj)
            {
                if (!modbus.Connected || !connected)
                    return inputs;
                try
                {
                    inputs = modbus.ReadDiscreteInputs(startingAddress + address, quantity);
                }
                catch (Exception)
                {
                    connected = false;
                }
            }
            return inputs;
        }


        public static int ReadSingleHoldingRegisters(int startingAddress)
        {
            int registers = 0;

            lock (obj)
            {
                if (!modbus.Connected || !connected)
                    return registers;
                try
                {
                    int[] registersArray = modbus.ReadHoldingRegisters(startingAddress + address, 1);

                    return registersArray[0];
                }
                catch (Exception)
                {
                    connected = false;
                }
            }
            return registers;
        }






        /// <summary>
        /// 读取寄存器
        /// </summary>
        /// <param name="startingAddress"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public static int[] ReadHoldingRegisters(int startingAddress, int quantity)
        {
            int[] registers = new int[quantity];

            lock (obj)
            {
                if (!modbus.Connected || !connected)
                    return registers;
                try
                {
                    registers = modbus.ReadHoldingRegisters(startingAddress + address, quantity);
                }
                catch (Exception)
                {
                    connected = false;
                }
            }
            return registers;
        }

        /// <summary>
        /// 读取输入寄存器
        /// </summary>
        /// <param name="startingAddress"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public static int[] ReadInputRegisters(int startingAddress, int quantity)
        {
            int[] registers = new int[quantity];
            lock (obj)
            {
                if (!modbus.Connected || !connected)
                    return registers;
                try
                {
                    registers = modbus.ReadInputRegisters(startingAddress + address, quantity);
                }
                catch (Exception)
                {
                    connected = false;
                }
            }
            return registers;
        }

        /// <summary>
        /// 读取写入多个寄存器
        /// </summary>
        /// <param name="startingAddressRead"></param>
        /// <param name="quantityRead"></param>
        /// <param name="startingAddressWrite"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static int[] ReadWriteMultipleRegisters(int startingAddressRead, int quantityRead, int startingAddressWrite, int[] values)
        {
            int[] registers = new int[quantityRead];
            lock (obj)
            {
                if (!modbus.Connected || !connected)
                    return registers;
                try
                {
                    registers = modbus.ReadWriteMultipleRegisters(startingAddressRead + address, quantityRead, startingAddressWrite + address, values);
                }
                catch (Exception)
                {
                    connected = false;
                }
            }
            return registers;
        }

        /// <summary>
        /// 写入多个线圈
        /// </summary>
        /// <param name="startingAddress"></param>
        /// <param name="values"></param>
        public static void WriteMultipleCoils(int startingAddress, bool[] values)
        {
            lock (obj)
            {
                if (!modbus.Connected) return;
                try
                {
                    modbus.WriteMultipleCoils(startingAddress + address, values);
                }
                catch (Exception ex)
                {
                    //写入失败不能把异常抛到UI线程，否则会弹出未处理异常导致程序崩溃
                    Logger.push(new Log(Logger.LogType.ERROR, "Modbus", "批量写线圈失败：" + startingAddress + " " + ex.Message));
                }
            }
        }

        /// <summary>
        /// 写入多个寄存器
        /// </summary>
        /// <param name="startingAddress"></param>
        /// <param name="values"></param>
        public static void WriteMultipleRegisters(int startingAddress, int[] values)
        {
            lock(obj)
            {
                if (!modbus.Connected) return;
                try
                {
                    modbus.WriteMultipleRegisters(startingAddress + address, values);
                }
                catch (Exception ex)
                {
                    Logger.push(new Log(Logger.LogType.ERROR, "Modbus", "批量写寄存器失败：" + startingAddress + " " + ex.Message));
                }
            }
        }

        /// <summary>
        /// 写入一个线圈
        /// </summary>
        /// <param name="startingAddress"></param>
        /// <param name="value"></param>
        public static void WriteSingleCoil(int startingAddress, bool value)
        {
            lock (obj)
            {
                if (!modbus.Connected) return;
                try
                {
                    modbus.WriteSingleCoil(startingAddress + address, value);
                }
                catch (Exception ex)
                {
                    //写线圈大多由按钮点击触发，异常抛到UI线程会直接崩溃
                    Logger.push(new Log(Logger.LogType.ERROR, "Modbus", "写线圈失败：" + startingAddress + " " + ex.Message));
                }
            }
        }
        /// <summary>
        /// 写入一个寄存器
        /// </summary>
        /// <param name="startingAddress"></param>
        /// <param name="value"></param>
        public static void WriteSingleRegister(int startingAddress, Int16 value)
        {
            lock(obj)
            {
                if (!modbus.Connected) return;
                try
                {
                    modbus.WriteSingleRegister (startingAddress + address, value);
                }
                catch (Exception ex)
                {
                    Logger.push(new Log(Logger.LogType.ERROR, "Modbus", "写寄存器失败：" + startingAddress + " " + ex.Message));
                }
            }
        }

        /// <summary>
        /// 读取浮点数
        /// </summary>
        /// <param name="startaddress"></param>
        /// <returns></returns>
        public static float ReadDoubleRegister(int startaddress)
        {
            float floatValue = 0;
            lock (obj)
            {
                if (!modbus.Connected || !connected)
                    return floatValue;
                try
                {
                    int[] registerValues = modbus.ReadHoldingRegisters(startaddress + address, 2);
                    floatValue = CombineToFloat(registerValues[0], registerValues[1]);
                }
                catch (Exception)
                {
                    connected = false;
                }
            }

            return floatValue;
        }
        /// <summary>
        /// 写入浮点数
        /// </summary>
        /// <param name="startaddress"></param>
        /// <param name="floatvalue"></param>
        public static void WriteRegisters(int startaddress, float floatvalue)
        {
            //float f = float.Parse(textBox1.Text);
            Byte[] bytes = new byte[4];
            bytes = BitConverter.GetBytes(floatvalue);
            Byte[] byteh = { 0x00, 0x00, 0x00, 0x00 };
            Byte[] byted = { 0x00, 0x00, 0x00, 0x00 };
            byted[0] = bytes[0];
            byted[1] = bytes[1];
            byteh[0] = bytes[2];
            byteh[1] = bytes[3];
            int result = BitConverter.ToInt32(byted, 0);
            int result1 = BitConverter.ToInt32(byteh, 0);
            int[] intValues = new int[2];
            intValues[0] = result;
            intValues[1] = result1;

            lock(obj)
            {
                if (modbus.Connected)
                    modbus.WriteMultipleRegisters(startaddress + address, intValues);
                else
                    return;
            }

        }

        public static void  WriteRegisters(int startaddress,int intvalue)
        {
            //int val = int.Parse(strValue);
            Byte[] bytes = new byte[4];
            bytes = BitConverter.GetBytes(intvalue);
            Byte[] byteh = { 0x00, 0x00, 0x00, 0x00 };
            Byte[] byted = { 0x00, 0x00, 0x00, 0x00 };
            byted[0] = bytes[0];
            byted[1] = bytes[1];
            byteh[0] = bytes[2];
            byteh[1] = bytes[3];
            int result = BitConverter.ToInt16(byted, 0);
            int result1 = BitConverter.ToInt16(byteh, 0);
            int[] intValues = new int[2];
            intValues[0] = result;
            intValues[1] = result1;

            lock (obj)
            {
                if (modbus.Connected)
                    modbus.WriteMultipleRegisters(startaddress + address, intValues);
                else
                    return;
            }
        }

        //新增

        #region // ============= 布尔值(线圈)操作 =============

        //Y/X线圈地址编号以八进制进行编制，长度为1024，读取线圈需注意读取方式，一次最多可以读取2000个线圈，汇川1024个小于2000
        //读线圈Y = modbusClient.ReadCoils(8000, 128); 
        //读线圈X = modbusClient.ReadDiscreteInputs(0, 128);


        /// <summary>
        /// 读取单个线圈状态
        /// </summary>
        /// <param name="startAddress">要读取的线圈地址</param>
        /// <returns>返回线圈的布尔值</returns>
        public static bool ReadCoil(int startAddress)
        {
            lock (obj)
            {
                try
                {
                    if (modbus.Connected)
                        return modbus.ReadCoils(startAddress + address, 1)[0];
                }
                catch { }
                return false;
            }
        }


        /// <summary>
        /// 批量读取线圈状态
        /// </summary>
        /// <param name="startingAddress">批量读取线圈的起始地址</param>
        /// <param name="count">需要读取线圈</param>
        /// <returns>返回布尔值数组</returns>
        public static bool[] ReadCoils2(int startingAddress, int count)
        {
            lock (obj)
            {
                try
                {
                    if (modbus.Connected)
                        return modbus.ReadCoils(startingAddress + address, count);
                }
                catch { }
                return new bool[count];
            }
        }


        /// <summary>
        /// 读取离散输入值（X线圈值）
        /// </summary>
        /// <param name="startingAddress">起始地址</param>
        /// <param name="count">读取线圈个数</param>
        /// <returns>返回线圈值的布尔类型数组</returns>
        public static bool[] ReadDiscreteInputCoils(int startingAddress, int count)
        {
            lock (obj)
            {
                try
                {
                    if (modbus.Connected)
                        return modbus.ReadDiscreteInputs(startingAddress + address, count);
                }
                catch { }
                return new bool[count];
            }
        }



        /// <summary>
        /// 写入单个线圈
        /// </summary>
        /// <param name="coilAddress">要写入线圈的地址</param>
        /// <param name="value">要写入线圈的布尔值</param>
        public static void WriteCoil(int coilAddress, bool value)
        {
            //原来缺锁：其它线程正在读同一个socket时会与本次写冲突(数据错乱/库内部异常)
            lock (obj)
            {
                if (!modbus.Connected || !connected) return;
                try
                {
                    //注意：参数名不能叫address，否则会遮蔽静态字段address(偏移量10000)
                    modbus.WriteSingleCoil(coilAddress + address, value);
                }
                catch (Exception ex)
                {
                    //原来抛异常，退出程序时PLC已断开会弹出未处理异常，改为只记日志
                    Logger.push(new Log(Logger.LogType.ERROR, "Modbus", "写线圈失败：" + coilAddress + " " + ex.Message));
                    connected = false;
                }
            }
        }


        /// <summary>
        /// 批量写入线圈
        /// </summary>
        /// <param name="startingAddress">批量写入线圈的起始地址</param>
        /// <param name="values">要写入线圈的布尔值数组</param>
        public static  void WriteCoils(int startingAddress, bool[] values)
        {
            //原来缺锁
            lock (obj)
            {
                if (!modbus.Connected || !connected) return;
                try
                {
                    modbus.WriteMultipleCoils(startingAddress + address, values);
                }
                catch (Exception ex)
                {
                    Logger.push(new Log(Logger.LogType.ERROR, "Modbus", "批量写线圈失败：" + startingAddress + " " + ex.Message));
                    connected = false;
                }
            }
        }


        #endregion


        #region // ============= 字符串操作 =============

        /// <summary>
        /// 读取字符串(包括中文)
        /// </summary>
        /// <param name="startingAddress">起始地址</param>
        /// <param name="byteLength">要读取的字符串的字节长度</param>
        /// <param name="isHoldingRegister">是否保持寄存器</param>
        /// <returns>返回读取到的字符串</returns>
        /// <exception cref="Exception"></exception>
        public static string ReadString(int startingAddress, int byteLength, bool isHoldingRegister = true)
        {
            try
            {
                // 每个寄存器2个字节，所以需要的寄存器数量是byteLength/2向上取整
                int registerCount = (int)Math.Ceiling(byteLength / 2.0);
                ushort[] registers;

                lock (obj)
                {
                    // 直接读取为ushort数组（关键修正）
                     registers = isHoldingRegister ?
                        Array.ConvertAll(modbus.ReadHoldingRegisters(startingAddress + address, registerCount), x => (ushort)x) :
                        Array.ConvertAll(modbus.ReadInputRegisters(startingAddress + address, registerCount), x => (ushort)x);
                }



                // 将寄存器值按大端(高字节在前)还原为字节数组，不依赖本机字节序
                byte[] bytes = new byte[registerCount * 2];
                for (int i = 0; i < registers.Length; i++)
                {
                    bytes[i * 2] = (byte)(registers[i] >> 8);        // 高字节在前
                    bytes[i * 2 + 1] = (byte)(registers[i] & 0xFF);  // 低字节在后
                }

                // 使用指定的编码方式转换为字符串，截取指定字节长度
                string result = Encoding.UTF8.GetString(bytes, 0, Math.Min(byteLength, bytes.Length));

                // 去除可能的空字符
                return result.Trim('\0');
            }
            catch (Exception ex)
            {
                throw new Exception($"读取字符串失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 写入字符串
        /// </summary>
        /// <param name="startingAddress"></param>
        /// <param name="value"></param>
        /// <param name="isHoldingRegister"></param>
        /// <exception cref="Exception"></exception>
        public static void WriteString(int startingAddress, string value, bool isHoldingRegister = true)
        {
            try
            {
                if (value.IsNullOrEmpty()) return;
                // 使用指定的编码方式将字符串转换为字节数组
                byte[] bytes = Encoding.UTF8.GetBytes(value);

                // 计算需要的寄存器数量(每个寄存器2字节)
                int registerCount = (int)Math.Ceiling(bytes.Length / 2.0);

                // 如果字节数为奇数，补0
                if (bytes.Length % 2 != 0)
                {
                    Array.Resize(ref bytes, bytes.Length + 1);
                    bytes[bytes.Length - 1] = 0;
                }

                // 转换为寄存器值数组：高字节在前(大端)，不依赖本机字节序
                int[] registers = new int[registerCount];
                for (int i = 0; i < registerCount; i++)
                {
                    registers[i] = (bytes[i * 2] << 8) | bytes[i * 2 + 1];
                }

                // 写入寄存器
                if (isHoldingRegister)
                {
                    lock (obj)
                    {
                        modbus.WriteMultipleRegisters(startingAddress + address, registers);
                    }
              
                }
                else
                {
                    throw new NotSupportedException("不能写入输入寄存器");
                }
            }
            catch (Exception ex)
            {
                //throw new Exception($"写入字符串失败: {ex.Message}");
            }
        }

        #endregion


        #region // ============= 浮点数操作 =============


        /// <summary>
        /// 读取浮点数(两个寄存器，4字节)
        /// </summary>
        /// <param name="startingAddress"> 起始地址</param>
        /// <param name="isHoldingRegister">是否保持寄存器</param>
        /// <returns>返回读到的浮点数</returns>
        /// <exception cref="Exception">抛出的异常</exception>
        public static float ReadFloat(int startingAddress, bool isHoldingRegister = true)
        {
            //原来缺锁：与其它线程的读写共用同一个socket会互相干扰
            lock (obj)
            {
                if (!modbus.Connected || !connected) return 0;
                try
                {
                    int[] registers = isHoldingRegister ?
                        modbus.ReadHoldingRegisters(startingAddress + address, 2) :
                        modbus.ReadInputRegisters(startingAddress + address, 2);

                    if (registers == null || registers.Length < 2) return 0;
                    return CombineToFloat(registers[0], registers[1]);
                }
                catch (Exception ex)
                {
                    Logger.push(new Log(Logger.LogType.ERROR, "Modbus", "读取浮点数失败：" + startingAddress + " " + ex.Message));
                    connected = false;
                    return 0;
                }
            }
        }


        /// <summary>
        /// 写入浮点数(两个寄存器)
        /// </summary>
        /// <param name="startingAddress">起始地址</param>
        /// <param name="value">要写入的浮点数</param>
        /// <param name="isHoldingRegister">是否保持寄存器</param>
        public static void WriteFloat(int startingAddress, float value, bool isHoldingRegister = true)
        {
            //原来缺锁
            lock (obj)
            {
                if (!modbus.Connected || !connected) return;
                try
                {
                    if (!isHoldingRegister)
                    {
                        Logger.push(new Log(Logger.LogType.ERROR, "Modbus", "不能写入输入寄存器"));
                        return;
                    }

                    int reg0, reg1;
                    SplitFromFloat(value, out reg0, out reg1);
                    modbus.WriteMultipleRegisters(startingAddress + address, new int[] { reg0, reg1 });
                }
                catch (Exception ex)
                {
                    Logger.push(new Log(Logger.LogType.ERROR, "Modbus", "写入浮点数失败：" + startingAddress + " " + ex.Message));
                    connected = false;
                }
            }
        }
        /// <summary>
        /// 批量读取浮点数
        /// </summary>
        /// <param name="startingAddress">起始地址</param>
        /// <param name="count">要读取的浮点数个数</param>
        /// <param name="isHoldingRegister">是否保持寄存器</param>
        /// <returns>读取到的浮点数数组</returns>
        public static float[] ReadFloats(int startingAddress, int count, bool isHoldingRegister = true)
        {
            //原来缺锁
            lock (obj)
            {
                if (!modbus.Connected || !connected) return new float[count];
                try
                {
                    int registerCount = count * 2;
                    int[] registers = isHoldingRegister ?
                        modbus.ReadHoldingRegisters(startingAddress + address, registerCount) :
                        modbus.ReadInputRegisters(startingAddress + address, registerCount);

                    //返回长度不足时按默认值处理，避免索引越界
                    if (registers == null || registers.Length < registerCount) return new float[count];

                    float[] result = new float[count];
                    for (int i = 0; i < count; i++)
                    {
                        result[i] = CombineToFloat(registers[i * 2], registers[i * 2 + 1]);
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    Logger.push(new Log(Logger.LogType.ERROR, "Modbus", "批量读取浮点数失败：" + startingAddress + " " + ex.Message));
                    connected = false;
                    return new float[count];
                }
            }
        }

        /// <summary>
        /// 批量写入浮点数
        /// </summary>
        /// <param name="startingAddress">起始地址</param>
        /// <param name="values">要写入的浮点数数组</param>
        /// <param name="isHoldingRegister">是否保持寄存器</param>
        public static void WriteFloats(int startingAddress, float[] values, bool isHoldingRegister = true)
        {
            //原来缺锁
            lock (obj)
            {
                if (!modbus.Connected || !connected) return;
                try
                {
                    if (!isHoldingRegister)
                    {
                        Logger.push(new Log(Logger.LogType.ERROR, "Modbus", "不能写入输入寄存器"));
                        return;
                    }

                    int registerCount = values.Length * 2;
                    int[] registers = new int[registerCount];

                    for (int i = 0; i < values.Length; i++)
                    {
                        int reg0, reg1;
                        SplitFromFloat(values[i], out reg0, out reg1);
                        registers[i * 2] = reg0;
                        registers[i * 2 + 1] = reg1;
                    }

                    modbus.WriteMultipleRegisters(startingAddress + address, registers);
                }
                catch (Exception ex)
                {
                    Logger.push(new Log(Logger.LogType.ERROR, "Modbus", "批量写入浮点数失败：" + startingAddress + " " + ex.Message));
                    connected = false;
                }
            }
        }

        #endregion

    }

}
