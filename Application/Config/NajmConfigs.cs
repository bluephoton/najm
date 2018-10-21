using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;
using System.Xml.Linq;
using System.Reflection;

namespace Najm.Config
{
    internal class NajmConfigs
    {
        #region Data members
        private static List<HandlerInfo> _handlersInfo;
        private static bool _isDirty;
        #endregion

        private static string ConfigFileLocation {
            get { return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "najm.config"); } 
        }

        internal static void Load()
        {
            XDocument d = XDocument.Load(ConfigFileLocation);
            
            // load handlers section
            LoadHandlers(d);

            _isDirty = false;
        }

        private static void LoadHandlers(XDocument d)
        {
            var handlers = GetNodesList(d, "Handlers", "Handler", "Config file has no handlers");

            _handlersInfo = new List<HandlerInfo>();
            
            foreach (var h in handlers)
            {
                LoadHandler(h);
            }
        }

        private static void LoadHandler(XElement handler)
        {
            if (handler != null)
            {
                Guid id = Guid.Parse(handler.Attribute("Id").Value);
                string location = handler.Attribute("Location").Value;
                string assembly = handler.Attribute("Assembly").Value;
                string param = handler.Attribute("Param").Value;
                bool isEnabled =  bool.Parse(handler.Attribute("IsEnabled").Value);

                _handlersInfo.Add(new HandlerInfo(id, location, assembly, param, isEnabled));
            }
        }

        private static IEnumerable<XElement> GetNodesList(XDocument d, string root, string nodeName, string exeption)
        {
            IEnumerable<XElement> handlers = null;
            var handlersRoot = d.Root.Element(root);
            if (handlersRoot != null)
            {
                handlers = handlersRoot.Elements(nodeName);
            }

            if (handlers == null)
            {
                throw new NajmException(exeption);
            }
            return handlers;
        }

        internal static void Save()
        {
            if (_isDirty)
            {
                _isDirty = false;
            }
        }

        internal static void DiscardChanges()
        {
            if (_isDirty)
            {
                Load();
                _isDirty = false;
            }
        }

        internal static IEnumerable<HandlerInfo> Handlers { get { return _handlersInfo; } }

        internal static void AddHandler(HandlerInfo hi)
        {
            _handlersInfo.Add(hi);
            _isDirty = true;
        }

        internal static void RemoveHandler(Guid id)
        {
            var item = _handlersInfo.Find(i => i.Id.Equals(id));
            if (item != null)
            {
                _handlersInfo.Remove(item);
                _isDirty = true;
            }
        }

        internal static void RemoveAll()
        {
            _handlersInfo.Clear();
            _isDirty = true;
        }

        internal static bool IsDirty { get { return _isDirty; } }

        internal static void EnableHandler(Guid id, bool isEnabled)
        {
            var item = _handlersInfo.Find(i => i.Id.Equals(id));
            if (item != null)
            {
                item.Enabled = isEnabled;
                _isDirty = true;
            }
        }
    }
}
