
using AnnotationImg.Views;
using System;
using System.Windows;
using System.Reflection;
using AnnotationImg.Domain;
using System.Configuration;

namespace AnnotationImg
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                var asm = Assembly.LoadFrom(ConfigurationManager.AppSettings["IImageInfoIntegratorAssemblyFile"]);
                var classType = asm.GetType(ConfigurationManager.AppSettings["IImageInfoIntegratorType"]);
                var integrator = Activator.CreateInstance(classType) as IImageInfoExporter;
                ImageInfoExporterFactory.Initialize(integrator);
            }
            catch (Exception ex)
            {
                // メッセージを出すだけ
                MessageBox.Show("出力プラグインの設定に失敗しました。" + Environment.NewLine + ex.Message);
            }

            var window = new AnnotationImgWindow();
            window.Show();
        }
    }
}
