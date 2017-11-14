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
import { connect } from 'react-redux';
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl';
import PTVCheckBox from "../../Components/PTVCheckBox";
import './Styles/PublishStatusContainer.scss';
import { Map } from 'immutable';
import ImmutablePropTypes from 'react-immutable-proptypes';
import PTVLabel from '../../Components/PTVLabel';
import shortId from 'shortid';
import { publishingStatuses } from './Enums'

/// Selectors
import { getPublishingStatuses, getPublishingStatusesImmutableList } from './Selectors';

export const publishingStatusMessages = defineMessages({
    publishStatusContainerStatusLabel: {
        id: "Containers.Common.PublishingStatus.Header.Title",
        defaultMessage: "Tila"
    },
    publishStatusContainerStatusLabelDraft: {
        id: "Containers.Common.PublishingStatus.Draft.Title",
        defaultMessage: "Luonnos"
    },
    publishStatusContainerStatusLabelPublished: {
        id: "Containers.Common.PublishingStatus.Published.Title",
        defaultMessage: "Julkaistu"
    },
    publishStatusContainerStatusLabelDeleted: {
        id: "Containers.Common.PublishingStatus.Deleted.Title",
        defaultMessage: "Arkistoitu"
    },
    publishStatusContainerStatusLabelModified: {
        id: "Containers.Common.PublishingStatus.Modified.Title",
        defaultMessage: "Muokattu"
    }
});

export const PublishStatusContainer = ({onChangeBoxCallBack, isSelectedSelector, keyToState, publishingStatuses, multiple,
    containerClass, labelClass, contentClass, publishingStatusLabel, tooltip, exclude, intl: { formatMessage }}) => {

    const handlePublishStatusChange = (id) => (checkbox) => {
        if (onChangeBoxCallBack)
        {
            onChangeBoxCallBack(id, checkbox.target.checked);
        }
    }

    const getCheckBoxes = (checkBoxes) => {

        return checkBoxes.map((object, i) => {
            if (exclude && Array.isArray(exclude) && exclude.indexOf(object.get('type')) !== -1) return null
            return getCheckBox(object, formatMessage, onChangeBoxCallBack)
        })
    }

    const getCheckBox = (checkBox, formatMessage, onCheckBoxClick) => {
        let id = multiple ? shortId.generate() : checkBox.get('id');
        let publishingStatusId = checkBox.get('id');
        let content;
        let type = 'default';

        switch(checkBox.get('type'))
        {
            case 0: {
                type = 'status-draft';
                content = formatMessage(publishingStatusMessages.publishStatusContainerStatusLabelDraft);
            }
            break;
            case 1: {
                type = 'status-published';
                content = formatMessage(publishingStatusMessages.publishStatusContainerStatusLabelPublished);
            }
            break;
            case 2: {
                type = 'status-deleted';
                content = formatMessage(publishingStatusMessages.publishStatusContainerStatusLabelDeleted);
            }
            break;
            // PTV-1553, PTV-1554 the modified state should behave like draft
            // case 3: {
            //     type = 'status-modified';
            //     content = formatMessage(publishingStatusMessages.publishStatusContainerStatusLabelModified);
            // }
            // break;
            default: {
            }
            break;
        }

        return (type !== 'default') ? <div key={id}>
                                      <PTVCheckBox key={id}
                                        id={id}
                                        publishingStatusId = {publishingStatusId}
                                        isSelectedSelector={ isSelectedSelector }
                                        chbType={type}
                                        keyToState={ keyToState }
                                        onClick={ handlePublishStatusChange(publishingStatusId) }
                                        >{content}</PTVCheckBox>
                                    </div>
                                         : null;
    }

    return(publishingStatuses && publishingStatuses.size > 0 ?
        <div className={containerClass}>
            <div className="row">
                <div className={labelClass}>
                    <PTVLabel tooltip={tooltip}>{publishingStatusLabel || formatMessage(publishingStatusMessages.publishStatusContainerStatusLabel)}</PTVLabel>
                </div>
                <div className={contentClass}>
                    <div className="chb-group inline">{getCheckBoxes(publishingStatuses)}</div>
                </div>
            </div>
        </div> : null)
};

PublishStatusContainer.propTypes = {
    onChangeBoxCallBack: PropTypes.func.isRequired,
    publishingStatuses: ImmutablePropTypes.list.isRequired,
    isSelectedSelector: PropTypes.func.isRequired,
    publishingStatusLabel: PropTypes.string,
    containerClass: PropTypes.string,
    labelClass: PropTypes.string,
    contentClass: PropTypes.string,
    exclude: PropTypes.array
};

PublishStatusContainer.defaultProps = {
    publishingStatusLabel: '',
    containerClass: ''
}

function mapStateToProps(state, ownProps) {
  return {
      publishingStatuses: getPublishingStatusesImmutableList(state)
  }
}

export default connect(mapStateToProps)(injectIntl(PublishStatusContainer));

const publishingStatusFormatter = (statusClass, message) => {
  return (
    <div className={statusClass}>
      {message}
    </div>
  )
}

const formatter = ({ publishingStatus, intl }) => {
  const publishingStatusNumber = publishingStatus.get('type')
  switch (publishingStatusNumber) {
    case publishingStatuses.draft:
    case publishingStatuses.modified: // PTV-1553, PTV-1554 the modified state should behave like draft
      return publishingStatusFormatter(
        'status-draft', intl.formatMessage(publishingStatusMessages.publishStatusContainerStatusLabelDraft
      )[0])
    case publishingStatuses.published:
      return publishingStatusFormatter(
        'status-published', intl.formatMessage(publishingStatusMessages.publishStatusContainerStatusLabelPublished
      )[0])
    case publishingStatuses.deleted:
      return publishingStatusFormatter(
        'status-deleted', intl.formatMessage(publishingStatusMessages.publishStatusContainerStatusLabelDeleted
      )[0])
    default:
      return publishingStatusFormatter(
        'status-draft', intl.formatMessage(publishingStatusMessages.publishStatusContainerStatusLabelDraft
      )[0])
  }
}

function mapStateToPropsFormatter (state, ownProps) {
  const publishingStatus = getPublishingStatuses(state).get(ownProps.cell) || Map()
  return {
      // publishingStatus: getPublishingStatuses(state).get(ownProps.cell) || Map()
    publishingStatus
  }
}

export const ColumnFormatter = connect(mapStateToPropsFormatter)(injectIntl(formatter))
