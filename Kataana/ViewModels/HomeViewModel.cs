using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Kataana.Models;

namespace Kataana.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public ProxyViewModel ProxyViewModel { get; set; }
        
        public HomeViewModel()
        {
            ProxyViewModel = new ProxyViewModel();
        }
    }
}
