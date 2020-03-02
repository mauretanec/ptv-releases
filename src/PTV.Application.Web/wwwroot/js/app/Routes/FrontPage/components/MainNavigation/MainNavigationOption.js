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
import { injectIntl, intlShape } from 'util/react-intl'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { mainNavigationSlugs } from 'enums'
import { getIsNavigationItemVisible } from 'Routes/FrontPage/selectors'
import styles from './styles.scss'

const MainNavigationOption = ({
  slug,
  message,
  isVisible,
  countSelector,
  count,
  intl: { formatMessage }
}) => (
  isVisible && (
    <div className={styles.option}>
      <span>{formatMessage(message)}</span>
      {countSelector && <span className={styles.optionCounter}>({count})</span>}
    </div>
  )
)

MainNavigationOption.propTypes = {
  intl: intlShape,
  slug: PropTypes.oneOf(mainNavigationSlugs),
  message: PropTypes.object.isRequired,
  isVisible: PropTypes.bool,
  count: PropTypes.number,
  countSelector: PropTypes.func
}

export default compose(
  injectIntl,
  connect((state, { domain, countSelector }) => ({
    isVisible: getIsNavigationItemVisible(state, { domain }),
    count: countSelector && countSelector(state)
  }))
)(MainNavigationOption)
