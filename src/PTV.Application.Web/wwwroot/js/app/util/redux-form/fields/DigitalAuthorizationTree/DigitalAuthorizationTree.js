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
import * as FintoSelectors from './selectors'
import { Field } from 'redux-form/immutable'
import { connect } from 'react-redux'
import { injectIntl, intlShape, defineMessages } from 'react-intl'
import { localizeItem } from 'appComponents/Localize'
import { SearchFilter, injectFormName, withLabel, withFormStates, asDisableable, asComparable } from 'util/redux-form/HOC'
import { TreeListDisplay } from 'util/redux-form/fields'
import { compose } from 'redux'
import { RenderTreeView } from 'util/redux-form/renders'
import {
  Nodes,
  Node,
  NodeLabelCheckBox,
  withCustomLabel,
  withLazyLoading
} from 'util/redux-form/renders/RenderTreeView/TreeView'
import { FintoSchemas } from 'schemas'
import * as nodeActions from 'Containers/Common/Actions/Nodes'
import mapDispatchToProps from 'Configuration/MapDispatchToProps'

const DigitalAuthorizationNodes = compose(
  connect((state, ownProps) => {
    const nodes = ownProps.searchValue &&
      FintoSelectors.getFilteredServiceClassIds(state, ownProps) ||
      FintoSelectors.getServiceClassIds(state, ownProps)
    return { nodes }
  })
)(Nodes)

const DigitalAuthorizationNode = compose(
  injectFormName,
  connect(
    (state, ownProps) => {
      const isSearch = ownProps.searchValue
      const selector = isSearch && FintoSelectors.getFilteredServiceClass || FintoSelectors.getServiceClass
      // const fromGD = FintoSelectors.getGeneralDescriptionServiceClassesIds(state, ownProps)
      // const isFromGD = fromGD.includes(ownProps.id)
      return {
        node: selector(state, ownProps),
        checked: ownProps.value && ownProps.value.get(ownProps.id) || false,
        // disabled: isFromGD,
        defaultCollapsed: !isSearch
      }
    },
    mapDispatchToProps([nodeActions])
  ),
  withLazyLoading({
    load: (props) => {
      props.actions.loadNodeChildren({
        treeNodeSchema: FintoSchemas.DIGITAL_AUTHORIZATION,
        treeType: 'DigitalAuthorization',
        node: props.node,
        contextId: props.contextId
      })
    }
  }),
  localizeItem({ input: 'node', output: 'node' }),
  withCustomLabel(NodeLabelCheckBox)
)(Node)

const messages = defineMessages({
  label: {
    id: 'Containers.Relations.ServiceAndChannel.ChannelRelation.DigitalAuthorization.Title',
    defaultMessage: 'Digitaalinen lupa'
  },
  tooltip: {
    id: 'Containers.Relations.ServiceAndChannel.ChannelRelation.DigitalAuthorization.Tooltip',
    defaultMessage: 'Digitaalinen lupa'
  },
  listLabel: {
    id: 'Containers.Relations.ServiceAndChannel.ChannelRelation.DigitalAuthorization.TargetList.Header',
    defaultMessage: 'Lisätyt tiedot'
  },
  info: {
    id: 'Containers.Relations.ServiceAndChannel.ChannelRelation.DigitalAuthorization.Info',
    defaultMessage: 'Notification text that user need to be check that selections are same as done under Digital authorizations'
  }
})

const ServiceClassTree = ({
  intl: { formatMessage },
  options,
  isReadOnly,
  ...rest
}) => (
  <div className='row'>
    <div className='col-lg-12 mb-2 mb-lg-0'>
      <Field
        name='digitalAuthorizations'
        component={RenderTreeView}
        NodesComponent={DigitalAuthorizationNodes}
        NodeComponent={DigitalAuthorizationNode}
        {...rest}
      />
    </div>
    <div className='col-lg-12 tree-list-adjustment'>
      <TreeListDisplay
        name='digitalAuthorizations'
        label={formatMessage(messages.listLabel)}
        selector={FintoSelectors.getServiceClassesForIdsJs}
        {...rest}
      />
    </div>
  </div>
)
ServiceClassTree.propTypes = {
  intl: intlShape,
  options: PropTypes.array,
  isReadOnly: PropTypes.bool.isRequired
}

const ServiceClassTreeComponent = compose(
  injectIntl,
  withLabel(messages.label, messages.tooltip),
  connect(null, {
    searchInTree: nodeActions.searchInTree,
    clearTreeSearch: nodeActions.searchInTree
  }),
  // SearchFilter.withSearchFilter({
  //   filterFunc: (props, value) => {
  //     if (value !== '') {
  //       props.searchInTree({
  //         treeNodeSchema: FintoSchemas.DIGITAL_AUTHORIZATION_ARRAY,
  //         treeType: 'DigitalAuthorization',
  //         value,
  //         contextId: props.contextId
  //       })
  //     } else {
  //       props.clearTreeSearch('DigitalAuthorization', props.contextId)
  //     }
  //   },
  //   partOfTree: true,
  //   componentClass: 'col-lg-12'
  // })
)(ServiceClassTree)

export default compose(
  injectIntl,
  asDisableable,
  withFormStates
)((props) =>
  (props.isReadOnly &&
    <TreeListDisplay
      name='digitalAuthorizations'
      label={props.intl.formatMessage(messages.label)}
      selector={FintoSelectors.getServiceClassesForIdsJs}
      {...props}
    /> || <ServiceClassTreeComponent {...props} />
  )
)
// export default compose(
//   injectIntl,
//   asComparable({ DisplayRender: (props) =>
//     <TreeListDisplay
//       name='digitalAuthorizations'
//       label={props.intl.formatMessage(messages.label)}
//       selector={FintoSelectors.getServiceClassesForIdsJs}
//       {...props}
//     />
//   }),
//   asDisableable,
//   withFormStates
// )(ServiceClassTreeComponent)
