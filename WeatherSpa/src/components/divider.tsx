import { HTMLAttributes, ReactNode } from 'react';
import './divider.css';

export type DividerVariant = 'default' | 'primary' | 'secondary';
export type DividerOrientation = 'horizontal' | 'vertical';

export interface DividerProps extends HTMLAttributes<HTMLDivElement> {
  /**
   * Optional text to display in the middle of the divider
   */
  children?: ReactNode;

  /**
   * Color variant of the divider
   */
  variant?: DividerVariant;

  /**
   * Orientation of the divider
   */
  orientation?: DividerOrientation;

  /**
   * Additional CSS class names
   */
  className?: string;
}

/**
 * Divider component for visual separation of content sections
 */
export function Divider({
  children,
  variant = 'default',
  orientation = 'horizontal',
  className = '',
  ...props
}: DividerProps) {
  const dividerClasses = [
    'divider',
    orientation === 'vertical' ? 'vertical' : '',
    variant !== 'default' ? variant : '',
    className,
  ]
    .filter(Boolean)
    .join(' ');

  return (
    <div className={dividerClasses} role="separator" {...props}>
      {children && <span>{children}</span>}
    </div>
  );
}
