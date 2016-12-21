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
import PTVTooltip from '../PTVTooltip';
import styles from './styles.scss';
import cx from 'classnames';

export const PTVLabel = props => {
  //const tooltipType = props.type;
  
  return (
    <label className={cx('ptv-label', { 'main': props.readOnly, 'with-tooltip': props.tooltip }, props.labelClass)} htmlFor={props.htmlFor}>
        <PTVTooltip
          readOnly={props.readOnly}
          tooltip={props.tooltip}
          type={props.type}
          labelContent={props.children}
          clearAll={props.clearAll}
          />
    </label>
  );
};

PTVLabel.propTypes = {
  tooltip: PropTypes.string,
  labelClass: PropTypes.string
};

PTVLabel.defaultProps = {
  htmlFor: '',
  tooltip: '',
  labelClass: ''
}

export default PTVLabel;
