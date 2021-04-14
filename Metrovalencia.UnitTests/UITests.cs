using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Metrovalencia.UnitTests
{
    [TestClass]
    public class UITests : SilverlightTest
    {
        #region *** MainPage Tests ***

        [TestMethod]
        public void CheckMainPageCreated()
        {
            var page = new Metrovalencia.MainPage();
            Assert.IsNotNull(page);
        }
/*
        [TestMethod]
        public void CheckSearchButton()
        {
            Metrovalencia.Helpers.MetroHelper mh = new Helpers.MetroHelper();
            mh.GetStops();
            var page = new Metrovalencia.MainPage();
        }*/

        #endregion


        #region *** FavoritePage Tests ***

        [TestMethod]
        public void CheckFavoritePageCreated()
        {
            var page = new Metrovalencia.FavoritePage();
            Assert.IsNotNull(page);
        }

        #endregion


        #region *** ImageViewer Tests ***

        [TestMethod]
        public void CheckImageViewerPageCreated()
        {
            var page = new Metrovalencia.ImageViewer();
            Assert.IsNotNull(page);
        }

        #endregion


        #region *** ResultsPage Tests ***

        [TestMethod]
        public void CheckResultsPageCreated()
        {
            var page = new Metrovalencia.ResultsPage();
            Assert.IsNotNull(page);
        }

        #endregion


        #region *** SelectStopPage Tests ***

        [TestMethod]
        public void CheckSelectStopPageCreated()
        {
            var page = new Metrovalencia.SelectStopPage();
            Assert.IsNotNull(page);
        }

        #endregion
    }
}
