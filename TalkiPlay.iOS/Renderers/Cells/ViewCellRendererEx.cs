using System;
using ChilliSource.Mobile.UI;
using TalkiPlay;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ViewCell), typeof(ViewCellRendererEx))]
namespace ChilliSource.Mobile.UI
{
    public class ViewCellRendererEx : ViewCellRenderer
    {

        public override void SetBackgroundColor(UITableViewCell tableViewCell, Cell cell, UIColor color)
        {
            base.SetBackgroundColor(tableViewCell, cell, color);
        }

        public override UITableViewCell GetCell(Cell pCell, UITableViewCell pReusableCell, UITableView pTableView)
        {
            UITableViewCell lCell = base.GetCell(pCell, pReusableCell, pTableView);

            if (lCell != null)
            {
                lCell.SelectionStyle = UITableViewCellSelectionStyle.None;
                SetBackgroundColor(lCell, pCell, UIColor.Clear);
                lCell.SelectionStyle = UITableViewCellSelectionStyle.None;
                lCell.BackgroundColor = UIColor.Clear;
            }

            return lCell;
        }
    }
}