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
import React, {Component, PropTypes} from 'react';
import { Link } from 'react-router';
import PTVNavigationItem from '../PTVNavigationItem';
import styles from './styles.scss';
import classNames from 'classnames';

export class PTVNavigation extends Component {

    constructor(props) {
        super(props);
        this.getMenuItems = this.getMenuItems.bind(this);
        this.setActiveMenuItem = this.setActiveMenuItem.bind(this);
        this.state = {
            activeMenuItemUid: this.props.defaultMenuItemUid
        }
    }

    setActiveMenuItem = (uid) => {
        this.setState({activeMenuItemUid: uid});
    }

    getMenuItems = ({menuItems, defaultMenuItemUid}) => {
        let self = this;
        return this.props.menuItems.map(function(menuItem) {
            return <PTVNavigationItem
                  active = {defaultMenuItemUid === menuItem.uid}
                  key = {menuItem.uid}
                  onSelect = {self.setActiveMenuItem}
                  uid = {menuItem.uid}
                  className = {classNames(menuItem.class)}
                  route = {menuItem.route}
                  routeText = {menuItem.routeText}
              />
        });
    }

    render() {
        return (
           <div id="header-content" className="container">
              <div className="header-row top-row">
                  <div className="container">
                    !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! TEMPORARY MENU !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                      <ul className="role-selection centered">
                          {this.getMenuItems(this.props)}
                      </ul>
                  </div>
              </div>
          </div>
        );
     }
 };

PTVNavigation.propTypes = {
  menuItems: PropTypes.array.isRequired,
  defaultMenuItemUid: PropTypes.string.isRequired
}

export default PTVNavigation;
