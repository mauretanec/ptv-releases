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
import React from 'react'
import PropTypes from 'prop-types'
import { Field, getFormInitialValues } from 'redux-form/immutable'
import { compose } from 'redux'
import { connect } from 'react-redux'
import HeaderFormatter from './HeaderFormater'
import { mergeInUIState } from 'reducers/ui'
import { createGetEntityAction } from 'Routes/actions'
import cx from 'classnames'
import styles from './styles.scss'
import CellHeaders from 'appComponents/CellHeaders'
import RenderChannelTableRow from './RenderChannelTableRow'
import withState from 'util/withState'
import AdditionalInformation from './AdditionalInformation'
import { getKey, formAllTypes } from 'enums'
import { injectFormName } from 'util/redux-form/HOC'
import { Map } from 'immutable'
import ImmutablePropTypes from 'react-immutable-proptypes'

const RenderChannelConnections = ({
  fields,
  mergeInUIState,
  updateUI,
  openedIndex,
  loadPreviewEntity,
  isReadOnly,
  initialValues
}) => {
  const handlePreviewOnClick = (id, channelCode) => {
    const formName = channelCode && getKey(formAllTypes, channelCode.toLowerCase())
    mergeInUIState({
      key: 'entityPreviewDialog',
      value: {
        sourceForm: formName,
        isOpen: true
      }
    })
    loadPreviewEntity(id, formName)
  }
  const tableCellFirstClass = cx(
    styles.tableCell,
    styles.tableCellFirst
  )
  return (
    <div className={styles.table}>
      <div className={styles.tableHead}>
        <div className={styles.tableRow}>
          <div className='row'>
            <div className='col-lg-3'>
              <div className={tableCellFirstClass}>
                <HeaderFormatter label={CellHeaders.languages} />
              </div>
            </div>
            <div className='col-lg-7'>
              <div className={styles.tableCell}>
                <HeaderFormatter label={CellHeaders.nameOrg} />
              </div>
            </div>
            <div className='col-lg-4'>
              <div className={styles.tableCell}>
                <HeaderFormatter label={CellHeaders.channelType} />
              </div>
            </div>
            <div className='col-lg-5'>
              <div className={styles.tableCell}>
                <HeaderFormatter label={CellHeaders.modifiedInfo} />
              </div>
            </div>
            <div className='col-lg-5'>
              <div className={styles.tableCell}>
                <HeaderFormatter label={CellHeaders.actions} />
              </div>
            </div>
          </div>
        </div>
      </div>
      <div className={styles.tableBody}>
        {fields && fields.map((field, index) => {
          const isOpen = openedIndex === index
          const handleOnRemove = () => fields.remove(index)
          const handleOnOpen = () => updateUI({
            openedIndex: isOpen
              ? null
              : index
          })
          const connectionRowClass = cx(
            styles.connectionRow,
            {
              [styles.isOpen]: isOpen
            }
          )
          const isAsti = initialValues.getIn([index, 'astiDetails', 'isASTIConnection']) || false
          return (
            <div key={index} className={connectionRowClass}>
              <Field
                name={field}
                component={RenderChannelTableRow}
                fields={fields}
                index={index}
                onClick={handlePreviewOnClick}
                onRemove={handleOnRemove}
                onOpen={handleOnOpen}
                isOpen={isOpen}
                isAsti={isAsti}
                isRemovable={!isReadOnly}
              />
              {isOpen &&
                <AdditionalInformation
                  index={index}
                  field={field}
                />}
            </div>
          )
        })}
      </div>
    </div>
  )
}
RenderChannelConnections.propTypes = {
  fields: PropTypes.array,
  mergeInUIState: PropTypes.func,
  updateUI: PropTypes.func,
  openedIndex: PropTypes.number,
  loadPreviewEntity: PropTypes.func,
  isReadOnly: PropTypes.bool,
  initialValues: ImmutablePropTypes.map.isRequired
}

export default compose(
  injectFormName,
  withState({
    initialState: {
      openedIndex: null
    }
  }),
  connect((state, { formName }) => ({
    initialValues: getFormInitialValues(formName)(state).get('selectedConnections') || Map()
  }), {
    mergeInUIState,
    loadPreviewEntity: createGetEntityAction
  })
)(RenderChannelConnections)
