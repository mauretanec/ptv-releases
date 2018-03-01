/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
import { defineMessages } from 'react-intl';
import { weekdaysEnum } from '../../../Common/Enums';

export const saveDraftMessages = defineMessages({
    text: {
        id: "Containers.Channel.ViewChannel.SaveDraft.Text",
        defaultMessage: "Luonnos on tallennettu. Voit julkaista kanavan sivulla olevasta Julkaise painikkeesta tai jatkaa Asiointikanavat -sivulle."
    },
    buttonOk: {
        id: "Containers.Channel.ViewChannel.SaveDraft.Accept",
        defaultMessage: "Ok"
    },
    buttonCancel: {
        id: "Containers.Channel.ViewChannel.SaveDraft.Cancel",
        defaultMessage: "Palaa Asiointikanavat -sivulle"
    }
});

export const goBackMessages = defineMessages({
  text: {
    id: 'Containers.Channel.ViewChannel.GoBack.Text',
    defaultMessage: 'Do you want to leave the page and lose all unsaved data?'
  },
  buttonOk: {
    id: 'Containers.Channel.ViewChannel.GoBack.Accept',
    defaultMessage: 'Kyllä'
  },
  buttonCancel: {
    id: 'Containers.Channel.ViewChannel.GoBack.Cancel',
    defaultMessage: 'Jatka muokkausta'
  }
})

export const formReceiverMessages = defineMessages({
    title: {
        id: "Containers.Channels.DeliveryAddress.FormReceiver.Title",
        defaultMessage: "Lomakkeen vastaanottaja"
    },
    tooltip: {
        id: "Containers.Channels.DeliveryAddress.FormReceiver.Tooltip",
        defaultMessage: "Lomakkeen vastaanottaja"
    },
    placeholder: {
        id: "Containers.Channels.DeliveryAddress.FormReceiver.Placeholder",
        defaultMessage: "paikkamerkkiteksti"
    }
});

export const deliveryAddressMessagges = defineMessages({
    title: {
        id: "Containers.Channels.DeliveryAddress.Title",
        defaultMessage: "Toimitusosoite"
    },
    descriptionTitle: {
        id: "Containers.Channels.DeliveryAddress.Description.Title",
        defaultMessage: "Sanallinen kuvaus"
    },
    descriptionTootltip: {
        id: "Containers.Channels.DeliveryAddress.Description.Tooltip",
        defaultMessage: "Sanallinen kuvaus"
    },
    descriptionPlaceholder: {
        id: "Containers.Channels.DeliveryAddress.Description.PlaceHolder",
        defaultMessage: "Kirjoita sanallinen kuvaus"
    },
    tooltip : {
        id: "Containers.Channels.DeliveryAddress.Street.Tooltip",
        defaultMessage: 'Kirjoita tarkka katuosoite ja tarkka osoitenumero erillisille kentille.'
    }
});

export const postalAddressMessagges = defineMessages({
    title: {
        id: "Containers.Cahnnels.PostalAddress.Title",
        defaultMessage: "Postiosoite (eri kuin käyntiosoite)"
    },
    tooltip : {
        id: "Containers.Channels.PostalAddress.Street.Tooltip",
        defaultMessage: 'Kirjoita tarkka katuosoite ja tarkka osoitenumero erillisille kentille.'
    }
});

export const visitingAddressMessagges = defineMessages({
    title: {
        id: "Containers.Cahnnels.VistingAddress.Title",
        defaultMessage: "Käyntiosoite"
    },
    tooltip : {
        id: "Containers.Channels.VistingAddress.Street.Tooltip",
        defaultMessage: 'Kirjoita tarkka katuosoite ja tarkka osoitenumero erillisille kentille.'
    }
});

export const openingHoursMessages = defineMessages({
    errorDayNotSelected: {
        id: "Containers.Channels.Common.OpeningHours.Error.DayNotSelected",
        defaultMessage: "Select at least one of the options above"
    },
    containsInvalidInfo: {
        id: "Containers.Channels.Common.OpeningHours.Error.InvalidInfo",
        defaultMessage: "Contains incomplete information"
    },
    openingHoursDailySetTitle: {
        id: "Containers.Channels.Common.OpeningHours.DailySet.Title",
        defaultMessage: "Normal service hours days"
    },
    dayTimesDate_normal_start: {
        id: "Containers.Channels.Common.OpeningHours.DayTimesStartDateNormal.Title",
        defaultMessage: "Normal start date"
    },
    dayTimesDate_normal_end: {
        id: "Containers.Channels.Common.OpeningHours.DayTimesEndDayNormal.Title",
        defaultMessage: "Normal end day"
    },
    dayTimesDate_exceptional_start: {
        id: "Containers.Channels.Common.OpeningHours.DayTimesStartDateExceptional.Title",
        defaultMessage: "Exceptional start date"
    },
    dayTimesDay_exceptional_start: {
        id: "Containers.Channels.Common.OpeningHours.DayTimesStartDayExceptional.Title",
        defaultMessage: "Exceptional start day"
    },
    dayTimesTime_exceptional_start: {
        id: "Containers.Channels.Common.OpeningHours.DayTimesStartTimeExceptional.Title",
        defaultMessage: "Exceptional start time"
    },
    dayTimesDate_exceptional_end: {
        id: "Containers.Channels.Common.OpeningHours.DayTimesEndDateExceptional.Title",
        defaultMessage: "Exceptional end date"
    },
    dayTimesDay_exceptional_end: {
        id: "Containers.Channels.Common.OpeningHours.DayTimesEndDayExceptional.Title",
        defaultMessage: "Exceptional end day"
    },
    dayTimesTime_exceptional_end: {
        id: "Containers.Channels.Common.OpeningHours.DayTimesEndTimeExceptional.Title",
        defaultMessage: "Exceptional end time"
    },
    dayTimesDate_special_start: {
        id: "Containers.Channels.Common.OpeningHours.DayTimesStartDateSpecial.Title",
        defaultMessage: "Special start date"
    },
    dayTimesDay_special_start: {
        id: "Containers.Channels.Common.OpeningHours.DayTimesStartDaySpecial.Title",
        defaultMessage: "Special start day"
    },
    dayTimesTime_special_start: {
        id: "Containers.Channels.Common.OpeningHours.DayTimesStartTimeSpecial.Title",
        defaultMessage: "Special start time"
    },
    dayTimesDate_special_end: {
        id: "Containers.Channels.Common.OpeningHours.DayTimesEndDateSpecial.Title",
        defaultMessage: "Special end date"
    },
    dayTimesDay_special_end: {
        id: "Containers.Channels.Common.OpeningHours.DayTimesEndDaySpecial.Title",
        defaultMessage: "Special end day"
    },
    dayTimesTime_special_end: {
        id: "Containers.Channels.Common.OpeningHours.DayTimesEndTimeSpecial.Title",
        defaultMessage: "Special end time"
    },
    showOpeningHours: {
        id: "Containers.Channels.Common.OpeningHours.ShowOpeningHours",
        defaultMessage: 'Palveluajat'
    },
    addBtnLabel: {
        id: "Containers.Channels.Common.OpeningHours.AddButtonLabel",
        defaultMessage: "Uusi aukioloaika"
    },
    collapsedInfo: {
        id: "Containers.Channels.Common.OpeningHours.CollapsedInfo",
        defaultMessage: "Muokattu"
    },
    invalidInfo: {
        id: "Containers.Channels.Common.OpeningHours.InvalidInfo",
        defaultMessage: "Contains incomplete information"
    },
    mainTooltipNormal: {
        id: "Containers.Channels.Common.OpeningHours.MainTooltipNormal",
        defaultMessage: "This is a tooltip for NOH"
    },
    mainLabelNormal: {
        id: "Containers.Channels.Common.OpeningHours.MainLabelNormal",
        defaultMessage: "Normaalitaukioloajat"
    },
    defaultTitle_openingHoursNormal: {
        id: "Containers.Channels.Common.OpeningHours.DefaultTitleNormal",
        defaultMessage: "Normaalitaukioloajat"
    },
    validOnward: {
        id: "Containers.Channels.Common.OpeningHours.ValidityType.Onward",
        defaultMessage: "Toistaiseksi voimassa oleva"
    },
    validPeriod: {
        id: "Containers.Channels.Common.OpeningHours.ValidityType.Period",
        defaultMessage: "Voimassa ajanjaksolla"
    },
    additionalInformation: {
        id: "Containers.Channels.Common.OpeningHours.AdditionalInformation",
        defaultMessage: "Lisätieto"
    },
    alternativeTitle: {
        id: "Containers.Channels.Common.OpeningHours.AlternativeTitle",
        defaultMessage: "Otsikko"
    },
    nonstopOpeningHours: {
        id: "Containers.Channels.Common.OpeningHours.Nonstop.Title",
        defaultMessage: "Aina avoinna"
    },
    startDate: {
        id: "Containers.Channels.Common.OpeningHours.StartDate.Title",
        defaultMessage: "Alkaa"
    },
    endDate: {
        id: "Containers.Channels.Common.OpeningHours.EndDate.Title",
        defaultMessage: "Päättyy"
    },
    mainTooltipSpecial: {
        id: "Containers.Channels.Common.OpeningHours.MainTooltipSpecial",
        defaultMessage: "This is a tooltip for SOH"
    },
    mainLabelSpecial: {
        id: "Containers.Channels.Common.OpeningHours.MainLabelSpecial",
        defaultMessage: "Vuorokauden yli menevät aukioloajat"
    },
    defaultTitle_openingHoursSpecial: {
        id: "Containers.Channels.Common.OpeningHours.DefaultTitleSpecial",
        defaultMessage: "Erityisjakso"
    },
    previewTitle: {
        id: "Containers.Channels.Common.OpeningHours.PreviewTitle",
        defaultMessage: "Esikatselu"
    },
    previewTooltip: {
        id: "Containers.Channels.Common.OpeningHours.PreviewTooltip",
        defaultMessage: "This is a tooltip for Preview"
    },
    previewInstructions1_openingHoursNormal: {
        id: "Containers.Channels.Common.OpeningHours.PreviewInstructions1Normal",
        defaultMessage: "Valitse viikonpäivät ja kellonajat, näet esikatselun lisättyäsi tietoa."
    },
    previewInstructions2_openingHoursNormal: {
        id: "Containers.Channels.Common.OpeningHours.PreviewInstructions2Normal",
        defaultMessage: "Jos aukioloajalle on voimassaoloaika, valitse 'Voimassa ajanjaksolla'."
    },
    previewInstructions1_openingHoursSpecial: {
        id: "Containers.Channels.Common.OpeningHours.PreviewInstructions1Special",
        defaultMessage: "Valitse viikonpäivät ja kellonajat, näet esikatselun lisättyäsi tietoa."
    },
    previewInstructions2_openingHoursSpecial: {
        id: "Containers.Channels.Common.OpeningHours.PreviewInstructions2Special",
        defaultMessage: "Jos aukioloajalle on voimassaoloaika, valitse 'Voimassa ajanjaksolla'."
    },
    previewInstructions1_openingHoursExceptional: {
        id: "Containers.Channels.Common.OpeningHours.PreviewInstructions1Exceptional",
        defaultMessage: "Valitse viikonpäivät ja kellonajat, näet esikatselun lisättyäsi tietoa."
    },
    previewInstructions2_openingHoursExceptional: {
        id: "Containers.Channels.Common.OpeningHours.PreviewInstructions2Exceptional",
        defaultMessage: "Jos aukioloajalle on voimassaoloaika, valitse 'Voimassa ajanjaksolla'."
    },
    [`weekday_${weekdaysEnum.monday}`]: {
        id: "Containers.Channels.OpeningHours.Monday",
        defaultMessage: "Maanantai"
    },
    [`weekday_${weekdaysEnum.tuesday}`]: {
        id: "Containers.Channels.OpeningHours.Tuesday",
        defaultMessage: "Tiistai"
    },
    [`weekday_${weekdaysEnum.wednesday}`]: {
        id: "Containers.Channels.OpeningHours.Wednesday",
        defaultMessage: "Keskiviikko"
    },
    [`weekday_${weekdaysEnum.thursday}`]: {
        id: "Containers.Channels.OpeningHours.Thursday",
        defaultMessage: "Torstai"
    },
    [`weekday_${weekdaysEnum.friday}`]: {
        id: "Containers.Channels.OpeningHours.Friday",
        defaultMessage: "Perjantai"
    },
    [`weekday_${weekdaysEnum.saturday}`]: {
        id: "Containers.Channels.OpeningHours.Saturday",
        defaultMessage: "Lauantai"
    },
    [`weekday_${weekdaysEnum.sunday}`]: {
        id: "Containers.Channels.OpeningHours.Sunday",
        defaultMessage: "Sunnuntai"
    },

    [`weekday_short_${weekdaysEnum.monday}`]: {
        id: "Containers.Channels.OpeningHours.Mo",
        defaultMessage: "Ma"
    },
    [`weekday_short_${weekdaysEnum.tuesday}`]: {
        id: "Containers.Channels.OpeningHours.Tu",
        defaultMessage: "Ti"
    },
    [`weekday_short_${weekdaysEnum.wednesday}`]: {
        id: "Containers.Channels.OpeningHours.We",
        defaultMessage: "Ke"
    },
    [`weekday_short_${weekdaysEnum.thursday}`]: {
        id: "Containers.Channels.OpeningHours.Th",
        defaultMessage: "To"
    },
    [`weekday_short_${weekdaysEnum.friday}`]: {
        id: "Containers.Channels.OpeningHours.Fr",
        defaultMessage: "Pe"
    },
    [`weekday_short_${weekdaysEnum.saturday}`]: {
        id: "Containers.Channels.OpeningHours.Sa",
        defaultMessage: "La"
    },
    [`weekday_short_${weekdaysEnum.sunday}`]: {
        id: "Containers.Channels.OpeningHours.Su",
        defaultMessage: "Su"
    },
    mainTooltipExceptional: {
        id: "Containers.Channels.Common.OpeningHours.MainTooltipExceptional",
        defaultMessage: "This is a tooltip for EOH"
    },
    mainLabelExceptional: {
        id: "Containers.Channels.Common.OpeningHours.MainLabelExceptional",
        defaultMessage: "Poikkeusaukioloajat"
    },
    defaultTitle_openingHoursExceptional: {
        id: "Containers.Channels.Common.OpeningHours.DefaultTitleExceptional",
        defaultMessage: "Poikkeusaukioloajat"
    },
    validDaySingle: {
        id: "Containers.Channels.Common.OpeningHours.ValidityType.DaySingle",
        defaultMessage: "Päivä"
    },
    validDayRange: {
        id: "Containers.Channels.Common.OpeningHours.ValidityType.DayRange",
        defaultMessage: "Ajanjakso"
    },
    closedDaySingle: {
        id: "Containers.Channels.Common.OpeningHours.ValidityType.ClosedDaySingle",
        defaultMessage: "Suljettu koko päivän"
    },
    closedDayRange: {
        id: "Containers.Channels.Common.OpeningHours.ValidityType.ClosedDayRange",
        defaultMessage: "Suljettu koko ajanjakson"
    },
    closedMessage: {
        id: "Containers.Channels.Common.OpeningHours.ValidityType.ClosedMessage",
        defaultMessage: "Suljettu"
    },
    openMessage: {
        id: "Containers.Channels.Common.OpeningHours.ValidityType.OpenMessage",
        defaultMessage: "Avoinna"
    }
});

export const channelServicesMessages = defineMessages({
    title: {
        id: "Containers.Channel.Common.ChannelServiceStep.Title",
        defaultMessage: "Liitetyt pavelut"
    },
    descriptionEdit: {
        id: "Containers.Channel.Common.ChannelServiceStep.Description.Edit.Title",
        defaultMessage: "Voit katsella, muokata ja lisätä niitä Liitokset sivulla."
    },
    descriptionAdd: {
        id: "Containers.Channel.Common.ChannelServiceStep.Description.Add.Title",
        defaultMessage: "Voit lisätä niitä Liitokset sivulla."
    },
    descriptionNumberOfConnectedServices: {
        id: "Containers.Channel.Common.ChannelServiceStep.Description.NumberOfConnectedServices.Title",
        defaultMessage: "Liitettyjen palvelujen lukumäärä on {serviceCount}."
    },
    descriptionZeroConnectedServices: {
        id: "Containers.Channel.Common.ChannelServiceStep.Description.ZeroConnectedServices.Title",
        defaultMessage: "Liitettyjen palvelujen lukumäärä on 0."
    },
    buttonEdit: {
        id: "Containers.Channel.Common.ChannelServiceStep.Edit.Button",
        defaultMessage: "Katsele ja muokkaa liitoksia"
    },
    buttonAdd: {
        id: "Containers.Channel.Common.ChannelServiceStep.Add.Button",
        defaultMessage: "Lisää palveluja"
    },
    serviceTableHeaderNameTitle:{
        id:"Containers.Channel.Common.ChannelServiceStep.Table.Name",
        defaultMessage:"Nimi"
    },
    serviceTableHeaderTypeTitle:{
        id:"Containers.Channel.Common.ChannelServiceStep.Table.Type",
        defaultMessage:"Palvelutyyppi"
    },
    serviceTableHeaderAttachedTitle:{
        id:"Containers.Channel.Common.ChannelServiceStep.Table.Attached",
        defaultMessage:"liitetty"
    },
    serviceTableHeaderAttachedByTitle:{
        id:"Containers.Channel.Common.ChannelServiceStep.Table.AttachedBy",
        defaultMessage:"liittäjä"
    }
});

export const areaInformationMessages = defineMessages({
  areaInformationTypeTitle: {
    id: 'Containers.Channels.Common.InformationArea.AreaInformationType.Title',
    defaultMessage: 'Täytä, jos alueen tiedot eroavat palvelun aluetiedoista.'
  },
  areaTypeTitle: {
    id: 'Containers.Channels.Common.InformationArea.Type.Title',
    defaultMessage: 'Aluetyyppi'
  },
  municipalityAreasTitle: {
    id: 'Containers.Channels.Common.InformationArea.Municipality.Title',
    defaultMessage: 'Valitse pääsääntöiset kunnat, joisaa organisaatio toimii:'
  },
  provinceAreasTitle: {
    id: 'Containers.Channels.Common.InformationArea.Province.Title',
    defaultMessage: 'Valitse pääsääntöiset maakunnat, joisaa organisaatio toimii:'
  },
  businessregionsAreasTitle: {
    id: 'Containers.Channels.Common.InformationArea.Business.Title',
    defaultMessage: 'Valitse pääsääntöiset Yrityspalvelujen seutualueet, joisaa organisaatio toimii:'
  },
  hospitalregionsAreasTitle: {
    id: 'Containers.Channels.Common.InformationArea.Hospital.Title',
    defaultMessage: 'Valitse pääsääntöiset Sairaanhoitopiirit, joisaa organisaatio toimii:'
  },
  selectedRegionsAreasTitle: {
    id: 'Containers.Channels.Common.InformationArea.Selected.Title',
    defaultMessage: 'Valinnat({count})'
  }
});


