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
import React, { PureComponent } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import cx from 'classnames'
import styles from './styles.scss'
import { PTVIcon } from 'Components'
import { Popup } from 'appComponents'
import { Button } from 'sema-ui-components'
import {
  withFormStates,
  injectFormName
} from 'util/redux-form/HOC'
import { injectIntl } from 'react-intl'
import withState from 'util/withState'
import { getIsAddingNewLanguage } from 'selectors/formStates'

const asContainer = ({ title,
  tooltip,
  simple,
  withCollection,
  uiKey,
  contentWhenCollapsed,
  collapsible = true,
  withoutState
}) => WrappedComponent => {
  class InnerComponent extends PureComponent {
    static propTypes = {
      className: PropTypes.string,
      isCollapsed: PropTypes.bool.isRequired,
      isReadOnly: PropTypes.bool.isRequired,
      onChange: PropTypes.func.isRequired,
      updateUI: PropTypes.func.isRequired,
      titleFromProps: PropTypes.object,
      tooltipFromProps: PropTypes.object,
      isDisabled: PropTypes.bool,
      isAddingNewLanguage: PropTypes.bool
    }
    handleOnToggle = () => {
      if (withoutState && this.props.onChange) {
        this.props.onChange(!this.props.isCollapsed)
      } else {
        this.props.updateUI('isCollapsed', !this.props.isCollapsed)
      }
    }
    WhenCollapsed = contentWhenCollapsed
    render () {
      let {
        isCollapsed,
        isReadOnly,
        titleFromProps,
        tooltipFromProps,
        isAddingNewLanguage,
        ...rest
      } = this.props
      if (!collapsible) {
        isCollapsed = false
      }
      const className = cx(
        styles.containerWrap,
        {
          [styles.isCollapsed]: isCollapsed,
          [styles.withCollection]: withCollection,
          [styles.isReadOnly]: isReadOnly,
          [styles.isAddingNewLanguage]: isAddingNewLanguage
        },
        this.props.className
      )
      const formatText = text =>
        text &&
        typeof text === 'object' &&
        text.id &&
        rest.intl.formatMessage(text) || text
      const containerTitle = titleFromProps || title
      const containerTooltip = tooltipFromProps || tooltip
      return (
        <div className={className}>
          {containerTitle &&
            <div className={styles.containerHead}>
              {isReadOnly &&
                <div className={styles.containerTitle}>{formatText(containerTitle)}</div> ||
                <Button
                  type='button'
                  onClick={this.handleOnToggle}
                  link
                  disabled={isAddingNewLanguage}
                >
                  {formatText(containerTitle)}
                </Button>
              }
              {collapsible && !isReadOnly && !isAddingNewLanguage &&
                <div className={styles.containerTitleActions}>
                  <PTVIcon onClick={this.handleOnToggle}
                    name={isCollapsed
                    ? 'icon-angle-down'
                      : 'icon-angle-up'}
                  />
                  {!isCollapsed && containerTooltip &&
                    <Popup
                      trigger={<PTVIcon name='icon-tip2' width={30} height={30} />}
                      content={formatText(containerTooltip)}
                    />
                  }
                </div>
              }
            </div>
          }
          {!isCollapsed
            ? <div className={styles.containerBody}>
              <WrappedComponent {...this.props} />
            </div>
            : this.WhenCollapsed && <this.WhenCollapsed {...this.props} /> || isReadOnly &&
              <WrappedComponent {...this.props} />
          }
        </div>
      )
    }
  }

  const HOCs = [
    injectIntl,
    injectFormName,
    withFormStates,
    connect((state, { title, tooltip, formName }) => ({
      titleFromProps: title,
      tooltipFromProps: tooltip,
      isAddingNewLanguage: getIsAddingNewLanguage(formName)(state)
    }))
  ]
  return (
    withoutState
      ? compose(...HOCs)
      : compose(
          ...HOCs,
          withState({
            redux: !!uiKey,
            key: uiKey,
            initialState: {
              isCollapsed: true
            }
          })
        )
  )(InnerComponent)
}

export default asContainer
