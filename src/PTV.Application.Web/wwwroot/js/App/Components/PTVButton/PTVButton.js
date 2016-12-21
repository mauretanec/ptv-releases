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
import React,  {Component, PropTypes} from 'react';
import './styles.scss'
import cx from 'classnames';
import { Link } from 'react-router';
import PTVIcon from '../PTVIcon';

export const PTVButton = (props) => {

  const handleClick = () => {
      props.onClick(props.item);
  };

  const {children, className, href, disabled,
    small, secondary, target, withIcon, iconName, type} = props;
    const clazz = cx({
      'small': small,
      'secondary': secondary,
      'icon': withIcon,
    }, 'ptv-'+type, className);
    
  const Element = href ? disabled ? 'label' : 'a' : type == 'link' ? 'a' : 'button';
  
  return (
    props.onClick ?
    <Element
      {...{href, target, disabled}}
      type="button"
      onMouseUp={handleClick}
      className={clazz}
    >
      {props.withIcon ?
        <PTVIcon width={20} height={20} name={props.iconName} /> : null} 
      {children}
    </Element> : href ?
    <Link className={clazz}
      to={href}>{children}</Link> : null
  );
};

PTVButton.propTypes = {
  onClick: PropTypes.func,
  children: PropTypes.any.isRequired,
  item: PropTypes.any,
  className: PropTypes.string,
  small: PropTypes.bool,
  disabled: PropTypes.bool,
  secondary: PropTypes.bool,
  href: PropTypes.string,
  target: PropTypes.string,
  withIcon: PropTypes.bool,
  iconName: PropTypes.string
};

PTVButton.defaultProps = {
  small: true,
  disabled: false,
  secondary: false,
  type: 'button'
}

export default PTVButton;
