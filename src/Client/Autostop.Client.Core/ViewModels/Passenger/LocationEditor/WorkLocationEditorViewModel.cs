﻿using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Autostop.Client.Abstraction.Models;
using Autostop.Client.Abstraction.Providers;
using Autostop.Client.Abstraction.Services;
using Autostop.Client.Core.Extensions;
using Autostop.Client.Core.Models;
using Autostop.Client.Core.ViewModels.Passenger.ChooseOnMap;
using Autostop.Client.Core.ViewModels.Passenger.LocationEditor.Base;

namespace Autostop.Client.Core.ViewModels.Passenger.LocationEditor
{
	public class WorkLocationEditorViewModel : BaseLocationEditorViewModel
	{
		private readonly IEmptyAutocompleteResultProvider _autocompleteResultProvider;

		public WorkLocationEditorViewModel(
			ISchedulerProvider schedulerProvider,
			INavigationService navigationService,
			IPlacesProvider placesProvider,
			IEmptyAutocompleteResultProvider autocompleteResultProvider,
			ISettingsProvider settingsProvider,
			IGeocodingProvider geocodingProvider) : base(schedulerProvider, placesProvider, geocodingProvider, navigationService)
		{
			_autocompleteResultProvider = autocompleteResultProvider;

			SelectedAutoCompleteResultModelObservable				
                .Subscribe(async result =>
				{
					var address = await geocodingProvider.ReverseGeocodingFromPlaceId(result.PlaceId);
					settingsProvider.SetWorkAddress(address);
					navigationService.GoBack();
				});

		    this.Changed(() => SelectedSearchResult)
		        .Where(r => r is SetLocationOnMapResultModel)
		        .Subscribe(result =>
		        {
		            navigationService.NavigateTo<ChooseWorkAddressOnMapViewModel>();
		        });
        }
        
		protected override ObservableCollection<IAutoCompleteResultModel> GetEmptyAutocompleteResult()
		{
			return new ObservableCollection<IAutoCompleteResultModel>
			{
				_autocompleteResultProvider.GetSetLocationOnMapResultModel()
			};
		}

		public override string PlaceholderText => "Set work address";
	}
}
