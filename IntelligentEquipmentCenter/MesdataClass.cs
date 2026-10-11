using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentEquipmentCenter
{
    public class MesdataBase
    {
        /// <summary>
        /// 数据类型 
        /// </summary>
        public string DataType;
    }


    #region 接收的信息


    /// <summary>
    /// 接收的心跳
    /// </summary>
    public class HeartRev : MesdataBase
    {
        public string DateTime;
        public string Sender;
        /// <summary>
        /// 客户端->服务端 Connected,服务端->客户端HeartBeat
        /// </summary>
        public string DataType;
        public int ClientId;
        public string Message;
        /// <summary>
        /// normal 正常  pause 暂停
        /// </summary>
        public string MesStatus;
    }

    //请求SN返回接口数据
    public class SnResult
    {
        /// <summary>
        /// SN数量
        /// </summary>
        public string SN;
        /// <summary>
        /// RequestSN表示下发SN
        /// </summary>
        public string DataType;
        public int ClientId;

        public string Sender;
        public string Result;

        public string MaterialCode;
        public string ParamVer;
    }
    /// <summary>
    /// 过站上传接口返回数据
    /// </summary>
    public class ResponseUploadSNResult
    {
        public string DateTime;
        /// <summary>
        /// SN数量
        /// </summary>
        public string Sender;
        /// <summary>
        /// ResponseUploadSNResult返回数据
        /// </summary>
        public string DataType;
        public int ClientId;
        public string Message;
        public string SN;
        public string Result;

    }




    #endregion


    #region  发送的信息
    /// <summary>
    /// 心跳信号
    /// </summary>
    public class HeartSend
    {
        public string DateTime;
        public string Sender;
        /// <summary>
        /// 客户端->服务端 Connected,服务端->客户端HeartBeat
        /// </summary>
        public string DataType;
        public int ClientId;
        public string Message;
        /// <summary>
        /// run 运行  free 停机  fault 故障  setup 调机
        /// </summary>
        public string DeviceStatus;
    }

    /// <summary>
    /// 客户端请求下发,返回的DataType=ResponseRequestSN
    /// </summary>
    public class SnRequest
    {
        /// <summary>
        /// SN数量
        /// </summary>
        public string Quantity;
        /// <summary>
        /// RequestSN表示下发SN
        /// </summary>
        public string DataType;
        public int ClientId;


    }


    //过站上传接口
    public class UploadSNResult
    {
        public string DateTime;
        public string Sender = "0001";
        /// <summary>
        /// SN数量
        /// </summary>
        public string SN;
        /// <summary>
        /// UploadSNResult上传接口
        /// </summary>
        public string DataType;
        public int ClientId;

        public string Message;
        public string Result;

        public string MaterialCode;
        public List<UploadSNResultParam> Data = new List<UploadSNResultParam>();
    }




    #endregion



    #region 弃用
    /// <summary>
    ///弃用   服务端主动下发SN,返回的DataType=ResponseDistributeSN
    /// </summary>
    public class SnSend
    {
        /// <summary>
        /// 物料编号
        /// </summary>
        public string MaterialCode;
        /// <summary>
        /// 物料名称
        /// </summary>
        public string MaterialName;
        /// <summary>
        /// DistributeSN表示下发SN,
        /// </summary>
        public string DataType;
        public List<string> SNList = new List<string>();
        public int ClientId;
        public string Message;

    }


    /// <summary>
    ///  弃用 下发设备参数和程序接口协议,返回参数DataType=ResponseDistributeDeviceParameters
    /// </summary>
    public class DeviceParameters
    {
        /// <summary>
        /// DistributeDeviceParameters表示下发设备参数
        /// </summary>
        public string DataType;
        /// <summary>
        /// 产品编号
        /// </summary>
        public string ProductCode;

        public int ClientId;

        /// <summary>
        /// 工艺参数JOSN
        /// </summary>
        public string DeviceParams;
    }



    /// <summary>
    /// 上传设备参数和程序接口协议,返回参数DataType=ResponseUploadDeviceParameters
    /// </summary>
    public class UpDeviceParameters
    {
        /// <summary>
        /// UploadDeviceParameters表示上传设备参数
        /// </summary>
        public string DataType;
        /// <summary>
        /// 产品编号
        /// </summary>
        public string ProductCode;

        public int ClientId;

        /// <summary>
        /// 工艺参数JOSN
        /// </summary>
        public string DeviceParams;

        /// <summary>
        /// 客户端文件路径数组
        /// </summary>
        public string[] FileUrls;
    }

    /// <summary>
    /// 上传产品SN进站时间接口协议,返回参数DataType=ResponseUploadInStationTime
    /// </summary>
    public class UpInStationTime
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceCode;
        /// <summary>
        /// 工位编号
        /// </summary>
        public string StationCode;
        /// <summary>
        /// UploadInStationTime上传进站时间
        /// </summary>
        public string DataType;

        public int ClientId;

        /// <summary>
        /// SN
        /// </summary>
        public string QrCode;
        /// <summary>
        ///进站时间
        /// </summary>
        public string InStationTime;
    }

    /// <summary>
    /// 设备反控接口协议,返回参数DataType=ResponseDevicePause
    /// </summary>
    public class DevicePause
    {
        /// <summary>
        /// DevicePause暂停，DeviceRun运行
        /// </summary>
        public string DataType;

        public int ClientId;

    }
    #endregion





    public class MesResult
    {
        public string DataType;
        /// <summary>
        /// 失败False，成功Success
        /// </summary>
        public string Result;
        public string Message;
        public int ClientId;
    }







    /// <summary>
    /// 上传结果的参数
    /// </summary>
    public class UploadSNResultParam
    {
        /// <summary>
        /// 参数编码
        /// </summary>
        public string paramCode;
        /// <summary>
        /// 参数名称
        /// </summary>
        public string paramName;
        /// <summary>
        /// 实际值
        /// </summary>
        public string RealValue;
        /// <summary>
        /// 加工结果，1：合格，0：不合格
        /// </summary>
        public string CheckResult;
    }






    /// <summary>
    /// 编码下发，配方切换,返回参数DataType=ResponseRecipeSwitchover
    /// </summary>
    public class RecipeSwitchover
    {
        /// <summary>
        /// RecipeSwitchover
        /// </summary>
        public string DataType;

        public int ClientId;
        /// <summary>
        /// 物料编号
        /// </summary>
        public string MaterialCode;
    }

    /// <summary>
    /// 不良信息上传,返回参数DataType=ResponseUploadDefective
    /// </summary>
    public class UpDefective
    {
        /// <summary>
        /// UploadDefective不良信息上传
        /// </summary>
        public string DataType;

        public int ClientId;
        /// <summary>
        /// sn
        /// </summary>
        public string SnNumber;

        public List<Defective> DataList = new List<Defective>();

    }

    public class Defective
    {
        /// <summary>
        /// 不良代码
        /// </summary>
        public string BadCode;
        /// <summary>
        /// 不良原因
        /// </summary>
        public string BadFactor;
        /// <summary>
        /// 不良数量
        /// </summary>
        public string BadQty;
        /// <summary>
        /// 不良时间
        /// </summary>
        public string EditTime;
    }

    /// <summary>
    /// 不良品SN上件接口协议,返回参数DataType=ResponseDefectiveSN
    /// </summary>
    public class DefectiveSN
    {
        /// <summary>
        /// DefectiveSN不良sn
        /// </summary>
        public string DataType;
        public int ClientId;
        public string Message;
        /// <summary>
        /// SN
        /// </summary>
        public string QrCode;
        /// <summary>
        /// 物料编号
        /// </summary>
        public string MaterialCode;
    }

    /// <summary>
    /// 设备状态上传接口协议,返回参数DataType=ResponseUploadEquipmentStatus
    /// </summary>
    public class UpEquipmentStatus
    {
        public string DateTime;
        public string Sender;
        /// <summary>
        /// UploadEquipmentStatus设备状态
        /// </summary>
        public string DataType;
        public int ClientId;
        public string Message;
        public DeviceStatu Data = new DeviceStatu();

    }

    /// <summary>
    /// 设备状态
    /// </summary>
    public class DeviceStatu
    {
        /// <summary>
        /// onMachine 上机 run 运行 free停机 fault故障 setup调机
        /// </summary>
        public string MachineStatusCode;
        public string MachineStatusBegin;
        public string MachineStatusEnd;

    }

    /// <summary>
    /// 设备报警上传接口协议,返回参数DataType=ResponseUploadAlarmMessage
    /// </summary>
    public class UpAlarmMessage
    {
        /// <summary>
        /// UploadAlarmMessage设备报警
        /// </summary>
        public string DataType;
        public int ClientId;
        public List<AlarmMessageT> Data = new List<AlarmMessageT>();

    }

    /// <summary>
    /// 设备报警
    /// </summary>
    public class AlarmMessageT
    {
        /// <summary>
        /// 报警ID
        /// </summary>
        public string AlarmId;
        /// <summary>
        /// 报警信息
        /// </summary>
        public string AlarmMesg;
        /// <summary>
        /// 报警状态 0：创建 1：关闭
        /// </summary>
        public int AlarmState;
        public string StartTime;
        public string EndTime;
    }


    /// <summary>
    /// 视觉检测图片上传接口协议,返回参数DataType=UploadVisualInspection
    /// </summary>
    public class UpVision
    {
        /// <summary>
        /// UploadVisualInspection视觉检测图片
        /// </summary>
        public string DataType;
        public int ClientId;
        public List<AlarmMessageT> Data = new List<AlarmMessageT>();

    }

    /// <summary>
    /// 视觉检测图片
    /// </summary>
    public class Vision
    {
        /// <summary>
        /// SN
        /// </summary>
        public string SN;
        /// <summary>
        /// 检测时间
        /// </summary>
        public string CheckTime;
        /// <summary>
        /// 检测结果 PASS FAIL
        /// </summary>
        public int CheckResult;

        /// <summary>
        /// 图片路径
        /// </summary>
        public string FilePath;

    }
    /// <summary>
    /// 设备托盘绑定信息上传
    /// </summary>
    public class UploadpalletInfo
    {
        public string DateTime;
        public string Sender;
        public string DataType;
        public int ClientId;
        public string Message;
        /// <summary>
        /// 
        /// </summary>
        public string PalletCode;
    }
    public class UploadpalletInfoResponse
    {
        public string DateTime;
        public string Sender;
        public string DataType;
        public int ClientId;
        public string Message;
        public string Result;
        public string SN;
    }

    /// <summary>
    /// 工艺参数 客户端响应
    /// </summary>
    public class ResponseRequestUploadDeviceParameters
    {
        public string DateTime;
        public string Sender = "";
        public string DataType;
        public int ClientId;
        public string Message = "";
        public string Result = "Success";
        public string MaterialCode = "";
        public string OperationCode = "";
        public string DeviceParams;
        public List<string> FilePaths = new List<string>();
    }

    /// <summary>
    ///工艺参数 服务端主动下发
    /// </summary>
    public class RequestUploadDeviceParameters
    {
        public string DateTime;
        public string Sender;
        public string DataType;
        public int ClientId;
        public string Message;
        public string MaterialCode;
        public string OperationCode;
    }



    /// <summary>
    /// 查询产品包装信息接口协议,返回参数DataType=ResponsePackageSearch
    /// </summary>
    public class PackageSearch
    {
        /// <summary>
        /// PackageSearch查询产品包装信息
        /// </summary>
        public string DataType;
        public int ClientId;
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceCode;
        /// <summary>
        /// 工位编号
        /// </summary>
        public string StationCode;

        /// <summary>
        /// SearchType为1时填SN号，为2时填包装箱号，
        /// </summary>
        public string SnNumber;
        /// <summary>
        /// 1,SN号，2包装箱号
        /// </summary>
        public string SearchType;

    }
    /// <summary>
    /// 查询产品包装信息返回信息类
    /// </summary>
    public class ResponsePackageSearch
    {

        /// <summary>
        /// ResponsePackageSearch响应包装信息查询
        /// </summary>
        public string DataType;

        public int ClientId;
        public string Message;
        /// <summary>
        /// Success结果
        /// </summary>
        public string Result;

        public string Data;
    }

    /// <summary>
    /// 查询产品包装信息返回信息类中的数据类
    /// </summary>
    public class PackageSearchparam
    {
        /// <summary>
        /// 工厂编码
        /// </summary>
        public string Factory;
        /// <summary>
        /// 工位编码
        /// </summary>
        public string StationCode;
        /// <summary>
        /// 包装箱号
        /// </summary>
        public string BarcodeNumber;
        /// <summary>
        /// 工单号
        /// </summary>
        public string OrderNumber;

        /// <summary>
        /// 比亚迪批次
        /// </summary>
        public string BatchNum;
        /// <summary>
        /// 物料编码
        /// </summary>
        public string MaterialCode;
        /// <summary>
        /// 物料名称
        /// </summary>
        public string MaterialName;

        /// <summary>
        /// 包装人
        /// </summary>
        public string PackageBy;
        /// <summary>
        /// sn
        /// </summary>
        public string QrCode;
        /// <summary>
        /// 日期
        /// </summary>
        public string ManufactureDate;
        /// <summary>
        /// 数量
        /// </summary>
        public string QtyUnit;
        /// <summary>
        /// 包装时间
        /// </summary>
        public string PackageTime;
        /// <summary>
        /// 
        /// </summary>
        public string VehicleType;
    }


    /// <summary>
    /// 加工工艺参数上传接口,返回参数DataType=ResponseUploadProductionParameters
    /// </summary>
    public class ProductionParameters
    {

        /// <summary>
        /// UploadProductionParameters上传生产参数
        /// </summary>
        public string DataType;
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceCode;
        /// <summary>
        /// 设备状态
        /// </summary>
        public string DeviceStatus;
        /// <summary>
        /// 工位编码
        /// </summary>
        public string StationCode;
        /// <summary>
        /// sn
        /// </summary>
        public string QrCode;

        /// <summary>
        /// 批次号
        /// </summary>
        public string LotCode;

        /// <summary>
        /// 1ok,0次品
        /// </summary>
        public int IsOK;

        /// <summary>
        /// 数量
        /// </summary>
        public int Num;
        /// <summary>
        /// 生产总量
        /// </summary>
        public int TotalNum;
        /// <summary>
        /// 总的合格数量
        /// </summary>
        public int TotalOkNum;
        /// <summary>
        /// 总的不合格数量
        /// </summary>
        public int TotalNoNum;
        /// <summary>
        /// 当天产量
        /// </summary>
        public int CurrNum;
        /// <summary>
        /// 当天合格数量
        /// </summary>
        public int CurrOkNum;
        /// <summary>
        /// 当天不合格数量
        /// </summary>
        public int CurrNoNum;

        public int ClientId;
        public List<ProducParameter> Data = new List<ProducParameter>();
    }

    /// <summary>
    /// 工艺参数
    /// </summary>
    public class ProducParameter
    {
        /// <summary>
        /// 参数编码
        /// </summary>
        public string ParamCode;
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParamName;
        /// <summary>
        /// 标准值
        /// </summary>
        public string StandardValue;

        /// <summary>
        /// 实际值"1.22,1.33等等"
        /// </summary>
        public string RealValue;

        /// <summary>
        /// 检测结果 0不合格，1合格
        /// </summary>
        public int CheckResult;
        /// <summary>
        /// 下限值
        /// </summary>
        public string ParamRange1;
        /// <summary>
        /// 上限值
        /// </summary>
        public string ParamRange2;
    }

    /// <summary>
    /// OCR识别镭雕信息上传
    /// </summary>
    public class LaserInfomation
    {
        /// <summary>
        /// UploadLaserInfomation识别镭雕信息上传
        /// </summary>
        public string DataType;
        public int ClientId;
        public List<AlarmMessageT> Data = new List<AlarmMessageT>();

    }

    /// <summary>
    /// 镭雕参数
    /// </summary>
    public class LaserParameter
    {
        /// <summary>
        /// sn
        /// </summary>
        public string Sn;
        /// <summary>
        /// 数字流水码
        /// </summary>
        public string FlowCode;
        /// <summary>
        /// 产品图号
        /// </summary>
        public string ProductNumber;

        /// <summary>
        /// 工厂代码
        /// </summary>
        public string PlantCode;

        /// <summary>
        /// 零部件代码
        /// </summary>
        public int ComponentCode;
        /// <summary>
        /// 产品状态“E”
        /// </summary>
        public string ProductStatus;
        /// <summary>
        /// 识别所有内容
        /// </summary>
        public string BarCode;
    }




}

