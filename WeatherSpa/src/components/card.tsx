import { forwardRef, HTMLAttributes, ReactNode } from 'react';
import './card.css';

export type CardVariant = 'default' | 'primary' | 'outline';

export interface CardProps extends HTMLAttributes<HTMLDivElement> {
  variant?: CardVariant;
  title?: string;
  hoverable?: boolean;
  className?: string;
  children: ReactNode;
}

export const Card = forwardRef<HTMLDivElement, CardProps>(
  ({ variant = 'default', title, hoverable = true, className = '', children, ...props }, ref) => {
    const cardClasses = [
      'ds-card',
      `ds-card-${variant}`,
      hoverable ? 'ds-card-hoverable' : '',
      className,
    ]
      .filter(Boolean)
      .join(' ');

    return (
      <div ref={ref} className={cardClasses} {...props}>
        {title && <h3 className="ds-card-title">{title}</h3>}
        <div className="ds-card-content">{children}</div>
      </div>
    );
  }
);

Card.displayName = 'Card';
