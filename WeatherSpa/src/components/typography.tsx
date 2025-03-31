import { HTMLAttributes, ReactNode, createElement, forwardRef } from 'react';
import './typography.css';

// Type for heading levels (h1-h6)
export type HeadingLevel = 1 | 2 | 3 | 4 | 5 | 6;

// Type for text alignment
export type TextAlignment = 'left' | 'center' | 'right';

// Type for text color variants
export type TextColor =
  | 'primary'
  | 'secondary'
  | 'tertiary'
  | 'brand'
  | 'success'
  | 'error'
  | 'warning'
  | 'info';

// Props for the Heading component
export interface HeadingProps extends HTMLAttributes<HTMLHeadingElement> {
  /**
   * Heading level (h1-h6)
   * @default 1
   */
  level?: HeadingLevel;

  /**
   * Text alignment
   */
  align?: TextAlignment;

  /**
   * Text color
   */
  color?: TextColor;

  /**
   * Whether to remove bottom margin
   * @default false
   */
  noMargin?: boolean;

  /**
   * Heading content
   */
  children: ReactNode;

  /**
   * Additional CSS class
   */
  className?: string;
}

/**
 * Heading component for all h1-h6 elements
 */
export const Heading = forwardRef<HTMLHeadingElement, HeadingProps>(
  ({ level = 1, align, color, noMargin = false, className = '', children, ...props }, ref) => {
    const tag = `h${level}` as 'h1' | 'h2' | 'h3' | 'h4' | 'h5' | 'h6';

    const headingClasses = [
      'ds-heading',
      `ds-heading-${level}`,
      align ? `ds-text-${align}` : '',
      color ? `ds-text-${color}` : '',
      noMargin ? 'ds-mb-0' : '',
      className,
    ]
      .filter(Boolean)
      .join(' ');

    return createElement(
      tag,
      {
        ref,
        className: headingClasses,
        ...props,
      },
      children
    );
  }
);

Heading.displayName = 'Heading';

// Type for text size variants
export type TextSize = 'xs' | 'sm' | 'md' | 'lg' | 'xl' | '2xl';

// Type for font weight variants
export type FontWeight = 'normal' | 'medium' | 'semibold' | 'bold';

// Type for line height variants
export type LineHeight = 'none' | 'tight' | 'normal' | 'relaxed';

// Props for the Text component
export interface TextProps extends HTMLAttributes<HTMLParagraphElement> {
  /**
   * Text size
   * @default 'md'
   */
  size?: TextSize;

  /**
   * Text alignment
   */
  align?: TextAlignment;

  /**
   * Text color
   */
  color?: TextColor;

  /**
   * Font weight
   */
  weight?: FontWeight;

  /**
   * Line height
   */
  lineHeight?: LineHeight;

  /**
   * Whether to add bottom margin
   * @default true
   */
  margin?: boolean;

  /**
   * Whether to use uppercase text
   * @default false
   */
  uppercase?: boolean;

  /**
   * Whether to apply underline
   * @default false
   */
  underline?: boolean;

  /**
   * Text content
   */
  children: ReactNode;

  /**
   * Additional CSS class
   */
  className?: string;
}

/**
 * Text component for paragraphs and other text elements
 */
export const Text = forwardRef<HTMLParagraphElement, TextProps>(
  (
    {
      size = 'md',
      align,
      color = 'secondary',
      weight,
      lineHeight = 'normal',
      margin = true,
      uppercase = false,
      underline = false,
      className = '',
      children,
      ...props
    },
    ref
  ) => {
    const textClasses = [
      'ds-text',
      size !== 'md' ? `ds-text-${size}` : '',
      align ? `ds-text-${align}` : '',
      color ? `ds-text-${color}` : '',
      weight ? `ds-font-${weight}` : '',
      lineHeight !== 'normal' ? `ds-leading-${lineHeight}` : '',
      !margin ? 'ds-mb-0' : '',
      uppercase ? 'ds-uppercase' : '',
      underline ? 'ds-underline' : '',
      className,
    ]
      .filter(Boolean)
      .join(' ');

    return (
      <p ref={ref} className={textClasses} {...props}>
        {children}
      </p>
    );
  }
);

Text.displayName = 'Text';
