﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using MyToolkit.Command;
using Onliner_for_windows_10.Login;
using Onliner_for_windows_10.Model;
using Onliner_for_windows_10.Model.DataTemplateSelector;
using Onliner_for_windows_10.ParsingHtml;

namespace Onliner_for_windows_10.View_Model
{
    public class ViewNewsPageViewModel : ViewModelBase
    {
        private ParsingFullNewsPage fullPagePars;
        private FullItemNews fullItem = new FullItemNews();
        private Request request = new Request();
        private string loaderPage = string.Empty;
        private string NewsID = string.Empty;

        #region Collections
        private ObservableCollection<ListViewItemSelectorModel> newsItem;
        private ObservableCollection<CommentsItem> commentsItem;

        public ObservableCollection<ListViewItemSelectorModel> NewsItemContent
        {
            get { return newsItem; }
            set { Set(ref newsItem, value); }
        }
        public ObservableCollection<CommentsItem> CommentsItem
        {
            get { return commentsItem; }
            set { Set(ref commentsItem, value); }
        }
        #endregion

        #region Properties
        private string _Value = "Default";
        public string LinkNews { get { return _Value; } set { Set(ref _Value, value); } }

        private bool progressRing = true;
        public bool ProgressRing
        {
            get
            {
                return this.progressRing;
            }
            set
            {
                Set(ref progressRing, value);
            }
        }

        private bool sendButton = false;
        public bool SendButton
        {
            get
            {
                return this.sendButton;
            }
            set
            {
                Set(ref sendButton, value);
            }
        }

        private bool commentsVisible = false;
        public bool CommentsVisible
        {
            get
            {
                return this.commentsVisible;
            }
            set
            {
                Set(ref commentsVisible, value);
            }
        }
        #endregion

        #region Commands
        public RelayCommand<object> CommentsAdd { get; private set; }
        public RelayCommand<object> OpenNewsInBrowser { get; private set; }
        public RelayCommand<object> SaveNewsInDB { get; private set; }
        public RelayCommand SendButtonActive { get; private set; }
        public RelayCommand ChangeCommetnsGridVisible { get; private set; }
        #endregion

        public ViewNewsPageViewModel()
        {
            CommentsAdd = new RelayCommand<object>((obj) => AddComments(obj));
            SendButtonActive = new RelayCommand(() => ActiveSendButton());
            ChangeCommetnsGridVisible = new RelayCommand(() => VisibleCommentsGrid());
            OpenNewsInBrowser = new RelayCommand<object>(async(obj) => await OpenLink(obj));
            SaveNewsInDB = new RelayCommand<object>((obj) => SaveNewsDB(obj));
        }

        private async Task LoadNewsData(string urlPage)
        {
            fullPagePars = new ParsingFullNewsPage(urlPage);
            NewsItemContent = new ObservableCollection<ListViewItemSelectorModel>(await fullPagePars.NewsMainInfo());
            CommentsItem = new ObservableCollection<Model.CommentsItem>(await fullPagePars.CommentsMainInfo());
        }

        private void VisibleCommentsGrid()
        {
            if (CommentsVisible)
            {
                CommentsVisible = false;
            }
            else
            {
                CommentsVisible = true;
            }
        }

        private void ActiveSendButton() =>
             SendButton = true;

        /// <summary>
        /// Open link in webBrowser
        /// </summary>
        /// <param name="obj">string url</param>
        /// <returns></returns>
        private async Task OpenLink(object obj) =>
            await Launcher.LaunchUriAsync(new Uri(obj.ToString()));
        
        /// <summary>
        /// Save item News in DB
        /// </summary>
        /// <param name="obj">News item</param>
        private async void SaveNewsDB(object obj)
        {
            MessageDialog msg = new MessageDialog("Coming soon");
            await msg.ShowAsync();
        }

        /// <summary>
        /// Add comments
        /// </summary>
        /// <param name="obj">comments data</param>
        private async void AddComments(object obj) =>
            await request.AddComments(fullPagePars.NewsID, obj.ToString());

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            LinkNews = (suspensionState.ContainsKey(nameof(LinkNews))) ? suspensionState[nameof(LinkNews)]?.ToString() : parameter?.ToString();
            await LoadNewsData(LinkNews);
            ProgressRing = false;
            await Task.CompletedTask;
        }
    }
}