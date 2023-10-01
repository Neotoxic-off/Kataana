using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Kataana.Models;

namespace Kataana.ViewModels
{
    public class AssemblyViewModel : BaseViewModel
    {
        private AssemblyModel _assembly;
        public AssemblyModel Assembly
        {
            get { return _assembly; }
            set { SetProperty(ref _assembly, value); }
        }

        private System.Reflection.AssemblyName ExecutingAssembly { get; set; }

        public AssemblyViewModel()
        {
            ExecutingAssembly = System.Reflection.Assembly.GetExecutingAssembly().GetName();
            Assembly = new AssemblyModel()
            {
                Name = GetAssemblyName(),
                Version = GetAssemblyVersion()
            };
        }

        private string GetAssemblyName()
        {
            return (ExecutingAssembly.Name);
        }

        private Version GetAssemblyVersion()
        {
            return (ExecutingAssembly.Version);
        }
    }
}
