import { HTMLAttributes, ReactNode } from 'react';
import { Button, ButtonProps, Card, CardProps } from './design-system';
import './card-link.css';

export interface CardLinkProps extends HTMLAttributes<HTMLDivElement> {
  /**
   * The title for the card
   */
  title: string;

  /**
   * The description text or content
   */
  description: ReactNode;

  /**
   * The action button text
   */
  actionText: string;

  /**
   * The URL to navigate to when clicked
   */
  to: string;

  /**
   * Button variant
   */
  buttonVariant?: ButtonProps['variant'];

  /**
   * Whether the button should take full width
   */
  fullWidthButton?: boolean;

  /**
   * Additional CSS class for the card
   */
  className?: string;

  /**
   * Card props to pass through
   */
  cardProps?: Partial<CardProps>;
}

/**
 * A card component with a title, description, and call-to-action button
 */
export function CardLink({
  title,
  description,
  actionText,
  to,
  buttonVariant = 'primary',
  fullWidthButton = true,
  className = '',
  cardProps,
  ...props
}: CardLinkProps) {
  const cardClassName = ['card-link', className].filter(Boolean).join(' ');

  return (
    <Card className={cardClassName} {...cardProps} {...props}>
      <h3 className="card-link-title">{title}</h3>
      <div className="card-link-description">
        {typeof description === 'string' ? <p>{description}</p> : description}
      </div>
      <div className="card-link-action">
        <Button to={to} variant={buttonVariant} fullWidth={fullWidthButton}>
          {actionText}
        </Button>
      </div>
    </Card>
  );
}
