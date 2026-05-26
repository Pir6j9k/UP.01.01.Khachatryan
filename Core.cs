using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace УП._01._01.Khachatryan
{
    public static class Core
    {
        public static ReadWriteDBEntities DB { get; set; } = new ReadWriteDBEntities();

        public static User CurrentUser { get; set; }

        public static Frame MainFrame { get; set; }

        public static bool CheckFrozen()
        {
            if (CurrentUser == null)
                return false;
            if (CurrentUser.IsFrozen)
            {
                MessageBox.Show("Ваш аккаунт заморожен");
                return true;
            }
            return false;
        }
    }
}
