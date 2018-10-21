using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Najm.Handlers.Integration;
using Najm.FITSIO;
using System.Windows.Forms;
using System.Reflection;
using Microsoft.Scripting.Hosting;
using IronPython.Runtime;
using IronPython.Hosting;

namespace IPython
{
    public class SessionData
    {
        public Object Object { get { return _object; } set { _object = value; } } 
        private object _object;
    }

    public class IronPythonHost : NajmHandler<SessionData>
    {
        public IronPythonHost()
        {
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // arg for python handler is the module
        public override void Load(string handlerModuleFile)
        {
            try
            {
                // ensure that module passed exist
                if (!File.Exists(handlerModuleFile))
                {
                    throw new PythonHandlerException("Code module file doesn't exist");
                }
                _handlerModuleFile = handlerModuleFile;

                // we'll create the python engine here
                _pythonEngine = Python.CreateEngine();

                // set path so import can locate modules
                _pythonEngine.SetSearchPaths(new string[] {Path.GetDirectoryName(handlerModuleFile)});

                // create module and import necessary namespaces
                _handlerModule = _pythonEngine.CreateModule("__NajmHandler@" + Guid.NewGuid());

                // import clr to make it available for pythong code provided
                // also add references to System.Windows.Forms, Najm.ImagingCore & Najm.FITSIO assemblies
                _pythonEngine.Execute("import clr", _handlerModule);
                _pythonEngine.Execute("clr.AddReference('System.Windows.Forms')", _handlerModule);
                _pythonEngine.Execute("clr.AddReference('System.Drawing')", _handlerModule);
                _pythonEngine.Execute("clr.AddReference('Najm.FITSIO')", _handlerModule);
                _pythonEngine.Execute("clr.AddReference('Najm.ImagingCore')", _handlerModule);
                
                // import required types
                _pythonEngine.Execute("from System.Windows.Forms import *", _handlerModule);
                _pythonEngine.Execute("from System.Drawing import *", _handlerModule);
                _pythonEngine.Execute("from Najm.FITSIO import *", _handlerModule);
                _pythonEngine.Execute("from Najm.ImagingCore import *", _handlerModule);
                _pythonEngine.Execute("from Najm.ImagingCore.ColorTables import *", _handlerModule);
                _pythonEngine.Execute("from Najm.ImagingCore.ColorScaling import *", _handlerModule);
                _pythonEngine.Execute("from Najm.ImagingCore.ColorMaps import *", _handlerModule);

                // TODO: check that it implements methods we expect
                // how?

                // bind our delegates to python methods
                BindMethods();


                // load and compile the module
                _pythonEngine.ExecuteFile(_handlerModuleFile, _handlerModule);

                // instantiate an object of NajmHandler class. It will be available in the module global namespaec
                _pythonEngine.Execute("g_nho = NajmHandler()", _handlerModule);
            }
            catch (System.Exception e)
            {
                ShowError(e);
            }

        }

        private void BindMethods()
        {
            _getID = AddPythonMethod<Func<string>>("getID", "def getID(): return g_nho.ID");
            _getName = AddPythonMethod<Func<string>>("getName", "def getName(): return g_nho.Name");
            _getToolstripImageName = AddPythonMethod<Func<string>>("getToolstripImageName", "def getToolstripImageName(): return g_nho.ToolstripImageName");
            _getTooltip = AddPythonMethod<Func<string>>("getTooltip", "def getTooltip(): return g_nho.Tooltip");
            _canHandle = AddPythonMethod<Func<IHDU[], bool>>("canHandle", "def canHandle(hdus): return g_nho.CanHandle(hdus)");
            _initialize = AddPythonMethod<Action<INajmHandlersManager, Panel>>("initialize", "def initialize(nhm, panel): g_nho.Initialize(nhm, panel)");
            _openSession = AddPythonMethod<Func<int, IHDU[], object>>("openSession", "def openSession(sid, hdus): return g_nho.OpenSession(sid, hdus)");
            _closeSession = AddPythonMethod<Action<object>>("closeSession", "def closeSession(sd): g_nho.CloseSession(sd)");
            _activateSession = AddPythonMethod<Action<object>>("activateSession", "def activateSession(sd): return g_nho.ActivateSession(sd)");
        }

        private T AddPythonMethod<T>(string methodName, string code)
        {
            ScriptSource src = _pythonEngine.CreateScriptSourceFromString(code, Microsoft.Scripting.SourceCodeKind.Statements);
            src.Execute(_handlerModule);
            return _handlerModule.GetVariable<T>(methodName);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region INajmHDUHandler Members (Sumply delegate all calls to python
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override Guid ID 
        { 
            get 
            {
                string id = Guid.Empty.ToString();
                try
                {
                    id = _getID();
                }
                catch (System.Exception e)
                {
                    ShowError(e);
                }
                return new Guid(id);
            }
        }
        public override string Name
        {
            get 
            {
                string name = "";
                try
                {
                    name = _getName();
                }
                catch (System.Exception e)
                {
                    ShowError(e);
                }
                return name;
            }
        }
        public override string ToolstripImageName
        {
            get
            {
                string toolstripImageName = "";
                try
                {
                    toolstripImageName = _getToolstripImageName();
                    // toolstrip image file path is relative to python code module path.
                    if(!Path.IsPathRooted(toolstripImageName))
                    {
                        toolstripImageName = Path.Combine(Path.GetDirectoryName(_handlerModuleFile), toolstripImageName);
                    }
                    toolstripImageName = "@" + toolstripImageName;
                }
                catch (System.Exception e)
                {
                    ShowError(e);
                }
                return toolstripImageName;
            }
        }
        public override string Tooltip
        {
            get
            {
                string tooltip = "";
                try
                {
                    tooltip = _getTooltip();
                }
                catch (System.Exception e)
                {
                    ShowError(e);
                }
                return tooltip;
            }
        }
        public override bool CanHandle(IHDU[] hdus)
        {
            bool can = false;
            try
            {
                can = _canHandle(hdus);
            }
            catch (System.Exception e)
            {
                ShowError(e);
            }
            return can;
        }
        public override void Initialize(INajmHandlersManager nhm, Panel panel)
        {
            // call python handler initialize
            try
            {
                _initialize(nhm, panel);
            }
            catch (System.Exception e)
            {
                ShowError(e);
            }
        }

        protected override SessionData OpenSession(int sid, IHDU[] hdus)
        {
            SessionData sd = new SessionData();
            try
            {
                sd.Object = _openSession(sid, hdus);
            }
            catch (System.Exception e)
            {
                ShowError(e);
                sd.Object = null;
            }
            return sd;
        }
        protected override void CloseSession(SessionData sd)
        {
            try
            {
                _closeSession(sd.Object);
            }
            catch (System.Exception e)
            {
                ShowError(e);
            }
        }
        protected override void ActivateSession(SessionData sd)
        {
            try
            {
                _activateSession(sd.Object);
            }
            catch (System.Exception e)
            {
                ShowError(e);
            }
        }
        #endregion

        private void ShowError(System.Exception e)
        {
            MessageBox.Show(e.Message);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region data members
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private ScriptEngine _pythonEngine;
        private ScriptScope _handlerModule;
        private string _handlerModuleFile;

        Func<string> _getID;
        Func<string> _getName;
        Func<string> _getToolstripImageName;
        Func<string> _getTooltip;
        Func<IHDU[], bool> _canHandle;
        Action<INajmHandlersManager, Panel> _initialize;
        Func<int, IHDU[], object> _openSession;
        Action<object> _closeSession;
        Action<object> _activateSession;

        #endregion
    }
}
