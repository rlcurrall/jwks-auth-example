import { HTMLAttributes, ReactNode, forwardRef } from 'react';
import './container.css';

export type ContainerSize = 'sm' | 'md' | 'lg' | 'fluid';
export type ContainerPadding = 'none' | 'sm' | 'md' | 'lg';

export interface ContainerProps extends HTMLAttributes<HTMLDivElement> {
  /**
   * Container size variant
   * @default 'md'
   */
  size?: ContainerSize;

  /**
   * Vertical padding size
   * @default 'none'
   */
  verticalPadding?: ContainerPadding;

  /**
   * Container content
   */
  children: ReactNode;

  /**
   * Additional CSS class
   */
  className?: string;
}

export const Container = forwardRef<HTMLDivElement, ContainerProps>(
  ({ size = 'md', verticalPadding = 'none', className = '', children, ...props }, ref) => {
    const containerClasses = [
      'ds-container',
      size !== 'md' ? `ds-container-${size}` : '',
      verticalPadding !== 'none' ? `ds-container-py-${verticalPadding}` : '',
      className,
    ]
      .filter(Boolean)
      .join(' ');

    return (
      <div ref={ref} className={containerClasses} {...props}>
        {children}
      </div>
    );
  }
);

Container.displayName = 'Container';
