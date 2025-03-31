import { HTMLAttributes, ReactNode } from 'react';
import { Button } from './design-system';
import './login-layout.css';

export interface LoginLayoutProps extends HTMLAttributes<HTMLElement> {
  /**
   * The title displayed at the top of the login form
   */
  title: string;

  /**
   * The main content of the login form
   */
  children: ReactNode;

  /**
   * Optional description text
   */
  description?: ReactNode;

  /**
   * Whether to show the back button
   */
  showBackButton?: boolean;

  /**
   * The URL to navigate to when the back button is clicked (defaults to "/login")
   */
  backUrl?: string;

  /**
   * The text for the back button (defaults to "Back to login options")
   */
  backText?: string;

  /**
   * Additional CSS class for the container
   */
  className?: string;
}

/**
 * A consistent layout for login-related pages
 */
export function LoginLayout({
  title,
  children,
  description,
  showBackButton = false,
  backUrl = '/login',
  backText = 'Back to login options',
  className = '',
  ...props
}: LoginLayoutProps) {
  const containerClasses = ['login-container', className].filter(Boolean).join(' ');

  return (
    <article className={containerClasses} {...props}>
      {showBackButton && (
        <div className="login-navigation">
          <Button to={backUrl} variant="text" size="sm">
            &larr; {backText}
          </Button>
        </div>
      )}

      <h1 className="login-page-title">{title}</h1>

      {description && (
        <div className="login-option-description">
          {typeof description === 'string' ? <p>{description}</p> : description}
        </div>
      )}

      <section className="login-form-section">{children}</section>
    </article>
  );
}
