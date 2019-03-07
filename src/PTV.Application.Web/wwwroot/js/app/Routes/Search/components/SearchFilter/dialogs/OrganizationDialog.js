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
import ModalDialog from 'appComponents/ModalDialog'
import styles from '../styles.scss'
import { messages } from '../messages'
import { Checkbox } from 'sema-ui-components'
import Spacer from 'appComponents/Spacer'
import { OrganizationSearchTree, ClearableFulltext } from 'util/redux-form/fields'
import { injectIntl, intlShape } from 'util/react-intl'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { connect } from 'react-redux'
import { getFormValueWithPath } from 'selectors/base'
import { OrderedSet } from 'immutable'
import { change } from 'redux-form/immutable'
import { searchInTree, clearTreeSearch } from 'Containers/Common/Actions/Nodes'
import { EntitySchemas } from 'schemas'

export const organizationDialogName = 'organizationDialog'

const OrganizationDialog = props => {
  const {
    intl: { formatMessage },
    dispatch,
    formName,
    isAllSelected,
    searchValue
  } = props

  const handleAllChange = () => {
    if (!isAllSelected) {
      dispatch(change(formName, 'organizationIds', OrderedSet()))
    }
  }

  const handleSearchChange = (input, value) => {
    if (!value || value.length < 3) {
      dispatch(clearTreeSearch('Organization'))
      return
    }

    dispatch(searchInTree({
      searchSchema: EntitySchemas.ORGANIZATION_ARRAY,
      treeType: 'Organization',
      value,
      contextId: value
    }))
  }

  return (
    <ModalDialog
      className={styles.searchFilterDialog}
      name={organizationDialogName}
      title={`${formatMessage(messages.commonSearchFilterTitle)}: ${formatMessage(messages.organizationFilterTitle)}`}
    >
      <div className={styles.dialogBody}>
        <Checkbox
          label={formatMessage(messages.allSelectedLabel)}
          onChange={handleAllChange}
          checked={isAllSelected}
          disabled={isAllSelected}
          className={styles.node}
        />
        <OrganizationSearchTree
          searchValue={searchValue}
          filterTree
          simple
          nodeClass={styles.node}
          containerClass={styles.nodeContainer}
        >
          <Spacer />
          <ClearableFulltext
            name='organizationSearch'
            placeholder={messages.organizationSearchPlaceholder}
            onChange={handleSearchChange}
            labelMessage={messages.commonSearchLabel}
            componentClass={styles.searchWrap}
            inputClass={styles.searchInput}
            iconClass={styles.searchIcon}
          />
        </OrganizationSearchTree>
      </div>
    </ModalDialog>
  )
}

OrganizationDialog.propTypes = {
  intl: intlShape,
  dispatch: PropTypes.func,
  formName: PropTypes.string,
  isAllSelected: PropTypes.bool,
  searchValue: PropTypes.string
}

export default compose(
  injectIntl,
  injectFormName,
  connect((state, ownProps) => {
    return {
      searchValue: getFormValueWithPath(_ => 'organizationSearch')(state, ownProps)
    }
  })
)(OrganizationDialog)
