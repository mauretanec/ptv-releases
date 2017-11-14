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
import { connect } from 'react-redux'
import { getOrganizationsByIds } from 'Routes/FrontPage/routes/Search/selectors'
import cx from 'classnames'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { localizeList } from 'appComponents/Localize'
import { compose } from 'redux'

const OrganizationCell = ({
  organizations,
  componentClass,
  compact
}) => {
  const organizationClass = cx(
    componentClass,
    'cell'
  )
  return (
    <div className={organizationClass}>
      {organizations.map((organization, index) => organization && <div key={index}>
        {organization.get && organization.get('name') || ''}</div>)}
    </div>
  )
}
OrganizationCell.propTypes = {
  organizations: ImmutablePropTypes.list.isRequired,
  componentClass: PropTypes.string,
  compact: PropTypes.bool
}
export default compose(
  connect(
  (state, { OrganizationIds }) => ({
    organizations: getOrganizationsByIds(OrganizationIds)(state)
  }
  )
),
  localizeList({
    input: 'organizations'
  }),
)(OrganizationCell)
