﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismMonkey.ViewModels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using Prism.Events;
    using Prism.Navigation;
    using Prism.Services;
    using Prism.Services.Dialogs;
    using PrismMonkey.Helpers;
    using PrismMonkey.Models;
    using PrismMonkey.Services;

    public class MonkeyDetailPageViewModel : INotifyPropertyChanged, INavigationAware
    {
        // 這裡是實作 INotifyPropertyChanged 介面需要用到的事件成員
        // 這是要用於屬性變更的時候，將會觸發這個事件通知
        public event PropertyChangedEventHandler PropertyChanged;

        #region 透過建構式注入的服務
        // 這是透過建構式注入的頁面導航的實作執行個體
        private readonly INavigationService navigationService;
        private readonly IPageDialogService dialogService;
        private readonly IMap map;
        #endregion

        #region 在此設計要進行資料綁定的屬性
        /// <summary>
        /// 要顯示的猴子明細物件
        /// </summary>
        public Monkey Monkey { get; set; } = new();
        #endregion

        #region 在此設計要進行命令物件綁定的屬性
        public DelegateCommand OpenMapCommand { get; set; }
        #endregion

        public MonkeyDetailPageViewModel(INavigationService navigationService,
            IPageDialogService dialogService, IMap map)
        {
            #region 將透過建構式注入進來的物件，指派給這個類別內的欄位或者屬性
            this.navigationService = navigationService;
            this.dialogService = dialogService;
            this.map = map;
            #endregion

            #region 在此將命令屬性進行初始化，建立命令物件與指派委派方法

            #region 根據猴子 GPS 位置，顯示此位置地圖在螢幕上
            OpenMapCommand = new DelegateCommand(async () =>
            {
                try
                {
                    // 開啟並且顯示地圖
                    await map.OpenAsync(Monkey.Latitude, Monkey.Longitude, new MapLaunchOptions
                    {
                        Name = Monkey.Name,
                        NavigationMode = Microsoft.Maui.ApplicationModel.NavigationMode.None
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Unable to launch maps: {ex.Message}");
                    await dialogService.DisplayAlertAsync("Error, no Maps app!", ex.Message, "OK");
                }
            });
            #endregion
            #endregion
        }

        #region 頁面導航將會觸發的方法
        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            #region 若有猴子物件傳送近來，則透過 parameter 將其取出來
            if (parameters.ContainsKey(ConstantHelper.NavigationKeyMonkey))
            {
                Monkey = parameters.GetValue<Monkey>(ConstantHelper.NavigationKeyMonkey);
            }
            else
            {
                Monkey = new();
            }
            #endregion
        }
        #endregion

    }
}
