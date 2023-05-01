using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace wpTest01.Logics
{
    internal class Commons
    {
        public static readonly string connString = "Data Source=localhost;" +
                                                   "Initial Catalog=AnimalHpt;" +
                                                   "User ID=sa;" +
                                                   "Password=12345";
        public static async Task<MessageDialogResult> ShowMessageAsync(string title, string message,
            MessageDialogStyle style = MessageDialogStyle.Affirmative)
        {
            return await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync(title, message, style, null);
        }
    }
}
