/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
import { injectIntl, intlShape } from 'util/react-intl'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { connect } from 'react-redux'
import { LanguageSearchTree } from 'util/redux-form/fields'
import { EnumsSelectors } from 'selectors'
import { OrderedSet } from 'immutable'
import { change } from 'redux-form/immutable'

export const languageDialogName = 'languageDialog'

const LanguageDialog = props => {
  const {
    intl: { formatMessage },
    dispatch,
    formName,
    availableLanguages,
    isAllSelected,
    nothingSelected
  } = props

  const handleAllChange = () => {
    const newValue = isAllSelected
      ? OrderedSet()
      : availableLanguages.map(value => value.get('code')).toOrderedSet()

    dispatch(change(formName, 'languages', newValue))
  }

  const getNothingError = () => {
    const title = formatMessage(messages.languageFilterTitle) || ''
    const prefix = formatMessage(messages.nothingSelectedPrefix)

    return `${prefix} ${title.toLowerCase()}.`
  }

  return (
    <ModalDialog
      className={styles.searchFilterDialog}
      name={languageDialogName}
      title={`${formatMessage(messages.commonSearchFilterTitle)}: ${formatMessage(messages.languageFilterTitle)}`}
      preventClosing={nothingSelected}
    >
      <div className={styles.dialogBody}>
        {nothingSelected &&
          <div className={styles.validationError}>
            {getNothingError()}
          </div>}
        <Checkbox
          label={formatMessage(messages.allSelectedLabel)}
          onChange={handleAllChange}
          checked={isAllSelected}
          className={styles.node}
        />
        <Spacer />
        <LanguageSearchTree
          filterTree
          nodeClass={styles.node}
          containerClass={styles.nodeContainer}
        />
      </div>
    </ModalDialog>
  )
}

LanguageDialog.propTypes = {
  intl: intlShape,
  dispatch: PropTypes.func,
  formName: PropTypes.string,
  isAllSelected: PropTypes.bool,
  availableLanguages: PropTypes.any,
  nothingSelected: PropTypes.bool
}

export default compose(
  injectIntl,
  injectFormName,
  connect((state, ownProps) => {
    return {
      availableLanguages: EnumsSelectors.translationLanguages.getEntities(state, ownProps)
    }
  })
)(LanguageDialog)
