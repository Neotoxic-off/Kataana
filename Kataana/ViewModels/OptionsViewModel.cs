using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Kataana.Models;
using Newtonsoft.Json;

namespace Kataana.ViewModels
{
    public class OptionsViewModel : BaseViewModel
    {
        public OptionsModel OptionsModel { get; set; }
        public DelegateCommand SaveOptionsCommand { get; set; }

        public OptionsViewModel()
        {
            OptionsModel = new OptionsModel()
            {
                OptionFile = "options.json"
            };
            SaveOptionsCommand = new DelegateCommand(SaveOptions);

            LoadOptions();
        }

        private void LoadOptions()
        {
            if (File.Exists(OptionsModel.OptionFile) == true)
            {
                OptionsModel.JSONOptionModel = JsonConvert.DeserializeObject<Models.JSON.JSONOptionModel>(
                    File.ReadAllText(
                        OptionsModel.OptionFile,
                        Encoding.UTF8
                    )
                );
            }
        }

        private void SaveOptions(object data)
        {
            if (File.Exists(OptionsModel.OptionFile) == true)
            {
                File.Delete(OptionsModel.OptionFile);
            }

            File.WriteAllText(
                OptionsModel.OptionFile,
                JsonConvert.SerializeObject(
                    OptionsModel.JSONOptionModel
                ),
                Encoding.UTF8
            );
        }
    }
}
