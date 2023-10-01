using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Kataana.Models;

namespace Kataana.ViewModels
{
    public class OptionsViewModel : BaseViewModel
    {
        public OptionsModel OptionsModel { get; set; }

        public OptionsViewModel()
        {
            OptionsModel = new OptionsModel();
        }
    }
}
