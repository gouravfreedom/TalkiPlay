//using System;
//using TalkiPlay.Shared;
//using Xamarin.Forms;

//namespace TalkiPlay
//{
//    public class TestBleRequestItemTemplateSelector : DataTemplateSelector
//    {
//        private readonly DataTemplate _testBleItemTemplate;
//        private readonly DataTemplate _emptyCellTemplate;

//        public TestBleRequestItemTemplateSelector()
//        {
//            _testBleItemTemplate = new DataTemplate(() => new TestBleRequstItemView());
//            _emptyCellTemplate = new DataTemplate(() => new SpacerCell());
//        }
        
//        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
//        {
//            if (item is IEmptyItemViewModel)
//            {
//                return _emptyCellTemplate;
//            }

//            if (item is ItemSelectionViewModel)
//            {
//                return _testBleItemTemplate;
//            }
            
//            throw new NotSupportedException();
//        }
//    }
//}