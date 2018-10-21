using System;
using System.Collections.Generic;
using System.Text;
using Najm.Handlers.Integration;
using System.Reflection;
using System.Drawing;
using System.IO;

namespace Najm.Handlers
{
    internal class HandlerAssembly
    {
        private const string DEFAULT_HANDLER_ID = "{37581060-646F-49aa-8D1E-1E255B81B471}";
        internal HandlerAssembly()
        {
        }

        internal void Load(string location, string handlerAssembly, string loadParam)
        {
            // any relative path will be handled as relative to Najm Bin directory.
            string binRoot = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(location) || !Path.IsPathRooted(location))
            {
                location = Path.Combine(binRoot, location); ;
            }
            // copy loacally for reload
            _handlerAssembly = Path.Combine(location, handlerAssembly);
            if (!string.IsNullOrEmpty(loadParam) && !Path.IsPathRooted(loadParam))
            {
                loadParam = Path.Combine(binRoot, loadParam);
            }
            _loadParam = loadParam;

            // load
            _hduHandler = LoadAssembly(_handlerAssembly, loadParam);
        }

        internal void Unload()
        {
            // do we need to do anything here?
        }

        internal void Reload()
        {
            Load("", _handlerAssembly, _loadParam);
        }

        private INajmHandler LoadAssembly(string assemblyName, string loadParam)
        {
            Assembly a = System.Reflection.Assembly.LoadFrom(assemblyName);
            INajmHandler hduHandler = null;
            Type[] expTypes = a.GetExportedTypes();
            foreach (Type t in expTypes)
            {
                if (t != null && t.IsClass && t.GetInterface("Najm.Handlers.Integration.INajmHandler") != null)
                {
                    hduHandler = (INajmHandler)a.CreateInstance(t.FullName);
                    
                    // add toolstrip button only if its not our default handler
                    if (hduHandler != null)
                    {
                        // call Load on the handler
                        hduHandler.Load(loadParam);

                        if (!hduHandler.ID.Equals(new Guid(DEFAULT_HANDLER_ID)))
                        {
                            // get toolstrip image if handler provide one
                            if (!String.IsNullOrEmpty(hduHandler.ToolstripImageName))
                            {
                                LoadToolstripImage(a, hduHandler.ToolstripImageName);
                            }

                            // get tooltip
                            _tooltip = hduHandler.Tooltip;
                        }
                        else
                        {
                            _default = true;
                        }
                        break;
                    }
                }
            }
            return hduHandler;
        }

        private void LoadToolstripImage(Assembly a, string imageResourceName)
        {
            Image i = null;
            Stream s;
            if (imageResourceName.StartsWith("@"))
            {
                // this is a file. Load image from it
                i = Image.FromFile(imageResourceName.Substring(1));
            }
            else
            {
                // it must be resource name then, act accordingly
                s = a.GetManifestResourceStream(imageResourceName);
                if (s != null)
                {
                    i = new Bitmap(s);
                }
            }
            _toolstripImage = i;
        }

        internal Image ToolstrupImage { get { return _toolstripImage; } }
        internal string Tooltip { get { return _tooltip; } }
        internal INajmHandler Handler { get { return _hduHandler; } }
        internal bool IsDefault { get { return _default; } }
        internal string Assembly { get { return _handlerAssembly; } }
        internal string LoadParam { get { return _loadParam; } }

        private string _handlerAssembly;
        private string _loadParam;
        private Image _toolstripImage;
        private string _tooltip;
        private INajmHandler _hduHandler;
        private bool _default;
    }
}
