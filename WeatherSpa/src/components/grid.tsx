import { HTMLAttributes, ReactNode, forwardRef } from 'react';
import './layout.css';

// Type for grid columns
export type GridColumns = 1 | 2 | 3 | 4;

// Type for responsive grid columns
export interface ResponsiveGridColumns {
  sm?: GridColumns;
  md?: GridColumns;
  lg?: GridColumns;
}

// Props for the Grid component
export interface GridProps extends HTMLAttributes<HTMLDivElement> {
  /**
   * Number of columns
   * @default 1
   */
  cols?: GridColumns;

  /**
   * Responsive columns configuration
   */
  responsive?: ResponsiveGridColumns;

  /**
   * Gap between items
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
 * Grid layout component for grid-based layouts
 */
export const Grid = forwardRef<HTMLDivElement, GridProps>(
  ({ cols = 1, responsive, gap = 'md', className = '', children, ...props }, ref) => {
    const responsiveClasses = [];

    if (responsive?.sm) {
      responsiveClasses.push(`ds-sm\\:grid-cols-${responsive.sm}`);
    }

    if (responsive?.md) {
      responsiveClasses.push(`ds-md\\:grid-cols-${responsive.md}`);
    }

    if (responsive?.lg) {
      responsiveClasses.push(`ds-lg\\:grid-cols-${responsive.lg}`);
    }

    const gridClasses = [
      'ds-grid',
      `ds-grid-cols-${cols}`,
      ...responsiveClasses,
      `ds-gap-${gap}`,
      className,
    ]
      .filter(Boolean)
      .join(' ');

    return (
      <div ref={ref} className={gridClasses} {...props}>
        {children}
      </div>
    );
  }
);

Grid.displayName = 'Grid';
