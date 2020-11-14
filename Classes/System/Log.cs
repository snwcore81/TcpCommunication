using System;
using System.Collections.Generic;
using System.Text;
using TcpCommunication.Classes.Exceptions;

namespace TcpCommunication.Classes.System
{
    public sealed class Log : IDisposable
    {
        private static readonly object       LockObject = new object();
        public static string        LineShift { get; set; }     = "└─────┐";
        public static string        LineBack  { get; set; }     = "┌─────┘";
        public static string        LinePrefix { get; set; }    = "│    ";
        public static ILogWriter    LogWriter { get; set; } = new ConsoleLogWriter();
        public static int           Iteration { get; private set; } = -1;
        public static List<string>  FullNameStack { get; private set; } = new List<string>();
        public static LevelEnum     CurrentLevel { get; set; } = Log.LevelEnum.DEB;

        public enum LevelEnum
        {
            DEB         = 0x0001,
            DET         = 0x0010,
            DEV         = 0x0100,
            GOD         = 0x1000
        }

        private readonly string         m_sClassName;
        private readonly string         m_sFuncName;
        private readonly LevelEnum      m_eLevel;
        private readonly bool           m_bLineShift = false;

        public Log(object a_oParentObject, string a_sFuncName, LevelEnum a_eLevel)
        {
            lock (LockObject)
            {
                m_sClassName = (a_oParentObject is string ? a_oParentObject.ToString() : a_oParentObject.GetType().Name.CleanType());
                m_sFuncName = a_sFuncName;

                FullNameStack.Add(FullName);

                if (a_eLevel <= CurrentLevel)
                {
                    m_bLineShift = true;

                    m_eLevel = a_eLevel;
                    Log.Iteration++;

                    Print($"[{FullName}]",true,a_eLevel);
                }
            }
        }

        public string FullName => $"{m_sClassName}::{m_sFuncName}";

        private string IteratedLineShift (bool a_bIsHeader)
        {
            string _sResult = "";

            if (Log.Iteration > 0)
            {
                int _iWhiteSpace = (Log.Iteration - 1) * (LineShift.Length-1);

                if (_iWhiteSpace > 0)
                    _sResult += new string(' ', _iWhiteSpace);

                _sResult += (a_bIsHeader ? LineShift : new string(' ', LineShift.Length-1));
            }

            if (!a_bIsHeader)
                _sResult += LinePrefix;

            return _sResult;
        }

        private string IteratedLineBack
        {
            get
            {
                string _sResult = "";

                if (Log.Iteration > 0)
                {
                    int _iWhiteSpace = (Log.Iteration - 1) * (LineBack.Length - 1);

                    if (_iWhiteSpace > 0)
                        _sResult += new string(' ', _iWhiteSpace);

                    _sResult += LineBack;
                }

                return _sResult;
            }
        }

        private string GenerateLogShiftLine(string a_sText, bool a_bIsHeader, LevelEnum a_eLevel)
        {
            return $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} <{a_eLevel}> {IteratedLineShift(a_bIsHeader)}{a_sText}" ;
        }

        private string GenerateLogBackLine(LevelEnum a_eLevel)
        {
            return $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} <{a_eLevel}> {IteratedLineBack}[{FullName}]";
        }

        private void Print (string a_sText, bool a_bIsHeader,LevelEnum a_eLevel)
        {
            if (a_eLevel <= Log.CurrentLevel)
            {
                LogWriter?.Write(GenerateLogShiftLine(a_sText, a_bIsHeader, a_eLevel));
            }
        }

        public void Dispose()
        {
            lock (LockObject)
            {
                if (m_bLineShift)
                {
                    LogWriter?.Write(GenerateLogBackLine(m_eLevel));
                    Log.Iteration--;
                }

                if (Log.Iteration >= 0)
                    FullNameStack.RemoveAt(Log.Iteration);
            }           
        }
       
        public void PR_DEB(string a_sText) => Print(a_sText, false, LevelEnum.DEB);        
        public void PR_DET(string a_sText) => Print(a_sText, false, LevelEnum.DET);
        public void PR_DEV(string a_sText) => Print(a_sText, false, LevelEnum.DEV);

        public static Log DEB(object a_oParentObject, string a_sFuncName) => new Log(a_oParentObject, a_sFuncName, LevelEnum.DEB);
        public static Log DET(object a_oParentObject, string a_sFuncName) => new Log(a_oParentObject, a_sFuncName, LevelEnum.DET);
        public static Log DEV(object a_oParentObject, string a_sFuncName) => new Log(a_oParentObject, a_sFuncName, LevelEnum.DEV);
    }
}
