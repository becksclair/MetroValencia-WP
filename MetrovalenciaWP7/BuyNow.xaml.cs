using System.Windows.Controls;
using System.Windows.Input;

namespace Metrovalencia
{
    public partial class BuyNow : UserControl
    {
        public BuyNow()
        {
            InitializeComponent();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Utils.PerformPurchase();
        }
    }
}
