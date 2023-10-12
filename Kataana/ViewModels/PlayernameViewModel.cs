using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Kataana.Models;

namespace Kataana.ViewModels
{
    public class PlayernameViewModel : BaseViewModel
    {
        public PlayernameModel PlayernameModel { get; set; }

        public PlayernameViewModel()
        {
            PlayernameModel = new PlayernameModel();
        }
    }
}
