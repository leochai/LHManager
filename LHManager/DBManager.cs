using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;

namespace LHManager
{
    public class DBManager
    {
        public static void Initial(OleDbConnection db, LHUnit[] unit)
        {
            OleDbCommand dbCmd = new OleDbCommand();
            dbCmd.Connection = db;
            dbCmd.CommandText = "select * from 单元状态 left join 试验记录 on 单元状态.试验编号 = 试验记录.试验编号";
            OleDbDataReader reader = dbCmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int i = Convert.ToInt32(reader["老化单元号"]) - 1;
                    //error
                    if (!(reader["器件类型"].Equals(DBNull.Value)))
                    {
                        unit[i].productBits = Convert.ToByte(reader["器件类型"]);
                    }
                    if (!(reader["运行状态"].Equals(DBNull.Value)))
                    {
                        unit[i].testingStatus = Convert.ToByte(reader["运行状态"]);
                    }
                    if (!(reader["单元地址"].Equals(DBNull.Value)))
                    {
                        unit[i].address = Convert.ToByte(reader["单元地址"]);
                    }
                    if (!(reader["最近上传时间"].Equals(DBNull.Value)))
                    {
                        unit[i].lastHour = (DateTime)reader["最近上传时间"];
                    }
                    if (!(reader["座子类型"].Equals(DBNull.Value)))
                    {
                        unit[i].seatType = Convert.ToBoolean(reader["座子类型"]);
                    }
                    if (!(reader["单元地址"].Equals(DBNull.Value)))
                    {
                        unit[i].testingStatus = Convert.ToByte(reader["单元地址"]);
                    }
                    if (!(reader["电压规格"].Equals(DBNull.Value)))
                    {
                        unit[i].voltageLevel = Convert.ToByte(reader["电压规格"]);
                    }
                    if (!(reader["上限"].Equals(DBNull.Value)))
                    {
                        unit[i].maxVoltage = Convert.ToByte(reader["上限"]);
                    }
                    if (!(reader["下限"].Equals(DBNull.Value)))
                    {
                        unit[i].minVoltage = Convert.ToByte(reader["下限"]);
                    }
                    if (!(reader["质量等级"].Equals(DBNull.Value)))
                    {
                        unit[i].qualityLevel = Convert.ToString(reader["质量等级"]);
                    }
                    if (!(reader["生产批号"].Equals(DBNull.Value)))
                    {
                        unit[i].productNum = Convert.ToString(reader["生产批号"]);
                    }
                    if (!(reader["标准号"].Equals(DBNull.Value)))
                    {
                        unit[i].standardNum = Convert.ToString(reader["标准号"]);
                    }
                    if (!(reader["单元状态.试验编号"].Equals(DBNull.Value)))
                    {
                        unit[i].testingNum = Convert.ToString(reader["单元状态.试验编号"]);
                    }
                    if (!(reader["产品型号"].Equals(DBNull.Value)))
                    {
                        unit[i].productType = Convert.ToString(reader["产品型号"]);
                    }
                    if (!(reader["开机日期"].Equals(DBNull.Value)))
                    {
                        unit[i].startDate = (DateTime)reader["开机日期"];
                    }
                    if (!(reader["操作员"].Equals(DBNull.Value)))
                    {
                        unit[i].testingOperator = Convert.ToString(reader["操作员"]);
                    }
                }
            }


        }

        public static void WriteResult(OleDbConnection db, string testNum, byte chipNum, byte num,
            DateTime time, double voltage, double power, double current)
        {
            OleDbCommand dbCmd = new OleDbCommand();
            dbCmd.Connection = db;

            time = time.AddMinutes(-time.Minute);
            time = time.AddSeconds(-time.Second);

            dbCmd.CommandText = "insert into 试验结果 values('"
                                + testNum + "',"
                                + chipNum + ","
                                + num + ",#"
                                + time + "#,"
                                + Math.Floor(voltage * 10) / 10 + ","
                                + Math.Floor(power * 10) / 10 + ","
                                + Math.Floor(current * 10) / 10 + ")";
            dbCmd.ExecuteNonQuery();
        }

        public static void UpdateLastHour(OleDbConnection db, byte unitNo, DateTime time)
        {
            OleDbCommand dbCmd = new OleDbCommand();
            dbCmd.Connection = db;
            dbCmd.CommandText = "update 单元状态 set 最近上传时间 =#" + time + "# where 老化单元号 = " + unitNo + 1;
            dbCmd.ExecuteNonQuery();
        }

        public static void UpdateStartTestingStatus(OleDbConnection db, LHUnit unit, byte unitNo)
        {
            OleDbCommand dbCmd = new OleDbCommand();
            dbCmd.Connection = db;
            dbCmd.CommandText = "update 单元状态 set 器件类型 = " + unit.productBits +
                                ", 运行状态 = 0, 最近上传时间 = #" + System.DateTime.Now +
                                "#, 试验编号 = '" + unit.testingNum +
                                "' where 老化单元号 = " + unitNo + 1;
            dbCmd.ExecuteNonQuery();
            dbCmd.CommandText = "insert into 试验记录 values('" + unit.testingNum +
                                "', '" + unit.productType +
                                "', '" + unit.productNum +
                                "', '" + unit.standardNum +
                                "', #" + unit.startDate +
                                "#, '" + unit.testingOperator +
                                "', '" + unit.qualityLevel +
                                "', " + unit.voltageLevel +
                                ", " + unit.maxVoltage +
                                ", " + unit.minVoltage + ")";
            dbCmd.ExecuteNonQuery();
            for (int i = 0; i < 96; i++)
            {
                dbCmd.CommandText = "update 对位表 set 器件编号 = " + unit.seatChart[i] +
                                    " where 老化单元号 = " + (unitNo + 1) +
                                    " and 老化位号 = " + (i + 1);
                dbCmd.ExecuteNonQuery();
            }
        }

        public static void UpdateRecallTime(OleDbConnection db)
        {
            OleDbCommand dbCmd = new OleDbCommand();
            dbCmd.Connection = db;
            GlobalElement.lastRecallTime = System.DateTime.Now;
            dbCmd.CommandText = "update 最近召回时间 set 时间 = #" + GlobalElement.lastRecallTime + "# where id = 1";
            dbCmd.ExecuteNonQuery();
        }

        public static void UpdateStatus(OleDbConnection db, byte unitNo, byte status)
        {
            if (status == 0x00 | status == 0x03 | status == 0x0C | status == 0x30)
            {
                OleDbCommand dbCmd = new OleDbCommand();
                dbCmd.Connection = db;
                dbCmd.CommandText = "update 单元状态 set 运行状态 = " + status + " where 老化单元号 = " + (unitNo + 1);
                dbCmd.ExecuteNonQuery();
            }
        }

        public static void UpdateReboot340Status(OleDbConnection db, LHUnit unit, byte unitNo)
        {
            OleDbCommand dbCmd = new OleDbCommand();
            dbCmd.Connection = db;
            dbCmd.CommandText = "update 单元状态 set 运行状态 = 0 where 老化单元号 = " + (unitNo + 1);
            dbCmd.ExecuteNonQuery();
            for (int i = 0; i < 96; i++)
            {
                dbCmd.CommandText = "update 对位表 set 器件编号 = " + unit.seatChart[i] +
                                    " where 老化单元号 = " + (unitNo + 1) +
                                    " and 老化位号 = " + (i + 1);
                dbCmd.ExecuteNonQuery();
            }
        }
    }
}
