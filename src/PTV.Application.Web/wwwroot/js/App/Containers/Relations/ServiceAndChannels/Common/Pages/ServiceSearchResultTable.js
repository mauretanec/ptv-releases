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
import React, {PropTypes, Component} from 'react';
import {connect} from 'react-redux';
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl';
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps';
import shortId from 'shortid';

//Actions
import * as commonServiceAndChannelActions from '../../Common/Actions';
import * as serviceSearchActions from '../../ServiceSearch/Actions';
import * as channelActions from '../../ChannelSearch/Actions';

// components
import { PTVLabel, PTVTable, PTVPreloader, PTVOverlay, PTVIcon } from '../../../../../Components';
import { ButtonShowMore } from '../../../../Common/Buttons';
import { ColumnFormatter } from '../../../../Common/PublishStatusContainer';
import ServiceContainer from '../../../../Services/Service/Pages/ServiceContainer';
import { ButtonCancel, ButtonContinue } from '../../../../Common/Buttons';
import ServiceAndChannelConfirmOverlay from '../../Common/Pages/ServiceAndChannelConfirmOverlay';
import TableNameFormatter from '../../../../Common/tableNameFormatter';

// selectors
import * as CommonSelectors from '../../../../Common/Selectors';
import * as ServiceSearchSelectors from '../Selectors';
import { getServices } from '../../../../Services/Service/Selectors';
import * as CommonServiceAndChannelSelectors from '../../Common/Selectors';

// messages
import commonMessages from '../../../../Common/LocalizedMessages';
import { detailMessages } from '../../ServiceSearch/Messages';
import { confirmationMessages } from '../../Common/Messages';

// styles
import * as overlayStyles from './OverlayStyles';

// types
import { confirmationTypes } from '../Helpers/confirmationTypes';

export const ServiceSearchResultTable =  ({messages, searchedServices, servicesCount, searchIsFetching, searchMaxPageCount, searchAreDataValid, searchIsMoreThanMax, pageNumber, selectedServicesIds, connectedServicesIdsMap, searchChannelsIsMoreThanMax,detailRow, confirmationData, keyToState, intl,language, actions }) => {            
    
    const { formatMessage } = intl;    
    
    const handleShowMore = () => {
        actions.loadServices(true);
    }

    const onAddService = (entity) => {               
        actions.onConnectedEntityAdd({ connectedServices: [ { uiId: shortId.generate(), ...entity }] });  
    }
      
    const onRemoveService = (id) => {
        const relationId = 'serviceAndChannelsId';
        const uiId = connectedServicesIdsMap.get(id);
        actions.onServiceAndChannelsListChange('connectedServices', relationId, uiId );  
    }    
    
    const onOneRowSelect= (row, isSelected, all) => {         
        if (isSelected) {              
            onAddService(row);  
        }
        else {           
            all ? onRemoveService(row.id)
                : actions.setRelationsConfirmation(confirmationTypes.UNSELECT_ONE_SERVICE, row.id);
        }
    }

    const onRowSelect = (row, isSelected) => {  
        onOneRowSelect(row, isSelected);
        if (isSelected)
        {
            //update channel search
            actions.loadChannelSearchResults();
        }
    }

    const onAllRowsSelect = (isSelected, rows) => {   
        
        if (isSelected)
        {            
            rows.forEach(row => onOneRowSelect(row,isSelected, true));                        
            //update channel search
            actions.loadChannelSearchResults();
        }
        else
        {
            const selectedRows = rows.filter(x => x !== undefined);
            actions.setRelationsConfirmation(confirmationTypes.UNSELECT_ALL_SERVICES, selectedRows.map(x => x.id));
        }
    } 

    const handleDetailIconClick = (row, event) =>{

        event.stopPropagation();
        actions.setRelationsDetail(row.id, keyToState, "service");                
    }

    const onDetailClose = () =>{
         actions.setRelationsDetail(null, keyToState, "service");
    }
    
    const onUnselectOneServiceAccept = (id) =>{
         onRemoveService(id); 
         actions.clearRelationsConfirmation();
         //update channel search
         actions.loadChannelSearchResults();
    }    
    
    const onUnselectAllServicesAccept = (rows) =>{
         rows.forEach(id => onRemoveService(id)); 
         actions.clearRelationsConfirmation();
         //update channel search
         actions.loadChannelSearchResults();
    }    
    
    const channelDetailIconFormatter = (cell,row) =>{
        return  <PTVIcon
                    onClick={(e) => handleDetailIconClick(row, e)}
                    name = "icon-info-circle"/>               
    }

    const generalFormatter = (cell,row) => <span>{ cell }</span>;
    const nameFormatter = (cell, row) => {
        return (<TableNameFormatter content={ cell } language= { language }/>);
    }
    const formatPublishingStatus = (cell,row) => {
        return <ColumnFormatter cell={cell} row={row} />;        
    }

    let selectRowProp = {
            mode: "checkbox",
            clickToSelect: true,
            //bgColor: "rgb(238, 193, 213)",
            className: "highlighted row-select",
            hideSelectColumn : false,
            // className: "row-select",
            onSelect: onRowSelect,
            onSelectAll: onAllRowsSelect,
            selected: selectedServicesIds 
        }
        
    const columnsDefinition = [
            {width:"50", dataField:"publishingStatus", dataFormat: formatPublishingStatus, columnHeaderTitle:formatMessage(messages.serviceSearchResultHeaderPublishingStatus)},
            {dataField:"id", isKey:true, hidden:true, columnHeaderTitle:'ID'},
            {dataField:"name", dataSort:true, dataFormat:nameFormatter, columnHeaderTitle:formatMessage(messages.serviceSearchResultHeaderName), columnClassName:"brand-color"},
            {width:"30", dataFormat:channelDetailIconFormatter},
        ];

    return (          
           <div className="form-group">          
           
            <ServiceAndChannelConfirmOverlay            
                isVisible = { confirmationData.has(confirmationTypes.UNSELECT_ONE_SERVICE)} 
                description = {formatMessage(confirmationMessages.serviceUnCheckTitle)}
                acceptCallback = { () => onUnselectOneServiceAccept(confirmationData.get(confirmationTypes.UNSELECT_ONE_SERVICE)) } 
            />
            <ServiceAndChannelConfirmOverlay            
                isVisible = { confirmationData.has(confirmationTypes.UNSELECT_ALL_SERVICES)} 
                description = {formatMessage(confirmationMessages.serviceUnCheckTitle)}
                acceptCallback = { () => onUnselectAllServicesAccept(confirmationData.get(confirmationTypes.UNSELECT_ALL_SERVICES)) }              
            />
                     
           <PTVOverlay
                title= {formatMessage(detailMessages.serviceDetailTitle)}
                dialogStyles= { overlayStyles.detailDialogStyles }
                overlayStyles = { overlayStyles.detailOverlayStyles }
                contentStyles = { overlayStyles.detailContentStyles }
                isVisible = { detailRow.size != 0 }
                onCloseClicked = { onDetailClose }
                >
                {detailRow.size != 0 ?<ServiceContainer simpleView = { true }/> : null }
            </PTVOverlay>
                  {searchedServices.size > 0 ?
                    <div>
                    {searchIsMoreThanMax ? <PTVLabel labelClass="strong">{formatMessage(messages.serviceResultCount) + (pageNumber*searchMaxPageCount) + '/' + servicesCount}</PTVLabel>
                        : <PTVLabel labelClass="strong">{formatMessage(messages.serviceResultCount) + servicesCount}</PTVLabel>}
                    <PTVTable maxHeight="450px"
                            contentDataSlector= { getServices }
                            data= { searchedServices }
                            striped={true}
                            hover={true}
                            pagination={false}
                            selectRow={selectRowProp}
                            tableHeaderClass="small"
                            tableBodyClass="small"
                            columnsDefinition={columnsDefinition} 
                            language = { language }/>
                    {searchIsMoreThanMax ? <div>
                                            <div className='button-group centered'>{ !searchIsFetching ? <ButtonShowMore onClick={handleShowMore}/> : <PTVPreloader />}</div>
                                            <PTVLabel labelClass="strong">{formatMessage(messages.serviceResultCount) + (pageNumber*searchMaxPageCount) + '/' + servicesCount}</PTVLabel>
                                            </div> :
                        <PTVLabel labelClass="strong">{formatMessage(messages.serviceResultCount) + servicesCount}</PTVLabel>}
                    </div>
                    : searchedServices.size == 0 && !searchIsFetching && searchAreDataValid ?
                        <PTVLabel labelClass="strong"> {formatMessage(commonMessages.noSeachResults)} </PTVLabel>
                        : searchIsFetching ?
                            <PTVPreloader />
                            : null
                }
            </div>
       );
}

ServiceSearchResultTable.propTypes = {
   
};

ServiceSearchResultTable.defaultProps = {
        keyToState: 'serviceAndChannelServiceSearch',
    };

function mapStateToProps(state, ownProps) { 
    const props = { keyToState: 'serviceAndChannelServiceSearch', keyToEntities: 'services' };     
  return {
    searchedServices: CommonSelectors.getSearchedEntities(state, props),
    servicesCount: CommonSelectors.getSearchResultsCount(state, props),
    searchIsFetching: CommonSelectors.getSearchResultsIsFetching(state, props),
    searchAreDataValid: CommonSelectors.getSearchResultsAreDataValid(state, props),
    searchIsMoreThanMax: CommonSelectors.getSearchResultsIsMoreThanMax(state, props),
    pageNumber: CommonSelectors.getSearchResultsPageNumber(state, props),
    searchMaxPageCount: CommonSelectors.getSearchResultsMaxPageCount(state, props),
    selectedServicesIds: CommonServiceAndChannelSelectors.getRelationConnectedServicesJS(state, props), 
    searchChannelsIsMoreThanMax: CommonSelectors.getSearchResultsIsMoreThanMax(state, props), 
    connectedServicesIdsMap: CommonServiceAndChannelSelectors.getRelationConnectedServicesIdsMap(state, props),
    detailRow:  CommonServiceAndChannelSelectors.getDetailService(state, props),
    confirmationData: CommonServiceAndChannelSelectors.getConfirmation(state, {keyToState:'serviceAndChannelConfirmation'}),
    language: CommonSelectors.getLanguageToCode(state, props)
  }
}

const actions = [
    commonServiceAndChannelActions,
    serviceSearchActions,
    channelActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceSearchResultTable));