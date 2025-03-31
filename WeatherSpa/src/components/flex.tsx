import { HTMLAttributes, ReactNode, forwardRef } from 'react';
import './layout.css';

// Type for flex direction
export type FlexDirection = 'row' | 'col';

// Type for justify content alignment
export type JustifyContent = 'start' | 'end' | 'center' | 'between' | 'around' | 'evenly';

// Type for align items
export type AlignItems = 'start' | 'end' | 'center' | 'baseline' | 'stretch';

// Props for the Flex component
export interface FlexProps extends HTMLAttributes<HTMLDivElement> {
  /**
   * Direction of flex items
   * @default 'row'
   */
  direction?: FlexDirection;

  /**
   * Justify content alignment
   */
  justify?: JustifyContent;

  /**
   * Align items
   */
  align?: AlignItems;

  /**
   * Whether to wrap items
   * @default false
   */
  wrap?: boolean;

  /**
   * Gap between items (uses CSS variables)
   */
  gap?: 'sm' | 'md' | 'lg' | 'xl';

  /**
   * Component children
   */
  children: ReactNode;

  /**
   * Additional CSS class
   */
  className?: string;
}

/**
 * Flex layout component for flexible layouts
 */
export const Flex = forwardRef<HTMLDivElement, FlexProps>(
  (
    { direction = 'row', justify, align, wrap = false, gap, className = '', children, ...props },
    ref
  ) => {
    const flexClasses = [
      'ds-flex',
      `ds-flex-${direction}`,
      justify ? `ds-justify-${justify}` : '',
      align ? `ds-items-${align}` : '',
      wrap ? 'ds-flex-wrap' : 'ds-flex-nowrap',
      gap ? `ds-gap-${gap}` : '',
      className,
    ]
      .filter(Boolean)
      .join(' ');

    return (
      <div ref={ref} className={flexClasses} {...props}>
        {children}
      </div>
    );
  }
);

Flex.displayName = 'Flex';
