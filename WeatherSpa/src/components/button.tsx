import React, { AnchorHTMLAttributes, ButtonHTMLAttributes, forwardRef } from 'react';
import { Link, LinkProps } from 'react-router-dom';
import './button.css';

export type ButtonVariant = 'primary' | 'secondary' | 'success' | 'error' | 'text' | 'nav-link';
export type ButtonSize = 'sm' | 'md' | 'lg';

// Base props shared by all button types
interface ButtonBaseProps {
  /**
   * Button visual variant
   * @default 'primary'
   */
  variant?: ButtonVariant;

  /**
   * Button size
   * @default 'md'
   */
  size?: ButtonSize;

  /**
   * Set to true to make the button take up the full width of its container
   * @default false
   */
  fullWidth?: boolean;

  /**
   * Optional CSS class name
   */
  className?: string;

  /**
   * Button content
   */
  children: React.ReactNode;
}

// Regular button props (no href or to)
export type ButtonProps = ButtonHTMLAttributes<HTMLButtonElement> & ButtonBaseProps;

// Link props with href
export type ButtonLinkProps = AnchorHTMLAttributes<HTMLAnchorElement> &
  ButtonBaseProps & {
    /**
     * URL for standard anchor link
     */
    href: string;

    /**
     * React Router to prop (should not be used with href)
     */
    to?: never;
  };

// React Router Link props with to
export type ButtonRouterProps = Omit<LinkProps, 'className'> &
  ButtonBaseProps & {
    /**
     * React Router navigation path
     */
    to: string;

    /**
     * Standard href (should not be used with to)
     */
    href?: never;
  };

// Union type to allow all three prop types
export type UnifiedButtonProps = ButtonProps | ButtonLinkProps | ButtonRouterProps;

// Type guards
const isRouterButtonProps = (props: UnifiedButtonProps): props is ButtonRouterProps =>
  'to' in props && props.to !== undefined;

const isLinkButtonProps = (props: UnifiedButtonProps): props is ButtonLinkProps =>
  'href' in props && props.href !== undefined;

/**
 * Unified Button component that renders:
 * - a regular button when neither href nor to is provided
 * - an anchor tag when href is provided
 * - a React Router Link when to is provided
 */
export const Button = forwardRef<HTMLButtonElement | HTMLAnchorElement, UnifiedButtonProps>(
  (props, ref) => {
    const buttonClasses = [
      'ds-button',
      `ds-button-${props.variant}`,
      `ds-button-${props.size}`,
      props.fullWidth ? 'ds-button-full' : '',
      props.className,
    ]
      .filter(Boolean)
      .join(' ');

    // React Router Link
    if (isRouterButtonProps(props)) {
      const {
        // eslint-disable-next-line @typescript-eslint/no-unused-vars
        className: _,
        children,
        to,
        ...routerRest
      } = props;
      return (
        <Link
          ref={ref as React.Ref<HTMLAnchorElement>}
          to={to}
          className={buttonClasses}
          {...routerRest}
        >
          {children}
        </Link>
      );
    }

    // Regular anchor link
    if (isLinkButtonProps(props)) {
      const { children, href, ...linkRest } = props;
      return (
        <a
          ref={ref as React.Ref<HTMLAnchorElement>}
          href={href}
          className={buttonClasses}
          {...linkRest}
        >
          {children}
        </a>
      );
    }

    // Regular button
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    const { children, className: _, ...rest } = props;
    return (
      <button ref={ref as React.Ref<HTMLButtonElement>} className={buttonClasses} {...rest}>
        {children}
      </button>
    );
  }
);

Button.displayName = 'Button';
