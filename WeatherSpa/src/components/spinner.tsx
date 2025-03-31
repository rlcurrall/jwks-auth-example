import { HTMLAttributes, forwardRef } from 'react';
import './spinner.css';

export type SpinnerSize = 'sm' | 'md' | 'lg';

export interface SpinnerProps extends HTMLAttributes<HTMLDivElement> {
  /**
   * Size of the spinner
   * @default 'md'
   */
  size?: SpinnerSize;

  /**
   * Text to display below the spinner
   */
  text?: string;

  /**
   * Whether to make the spinner fill the container height
   * @default false
   */
  fullHeight?: boolean;

  /**
   * Whether to make the spinner fill the screen height
   * @default false
   */
  fullScreen?: boolean;

  /**
   * Additional CSS class
   */
  className?: string;
}

/**
 * Spinner component for loading states
 */
export const Spinner = forwardRef<HTMLDivElement, SpinnerProps>(
  (
    { size = 'md', text, fullHeight = false, fullScreen = false, className = '', ...props },
    ref
  ) => {
    const containerClasses = [
      'ds-spinner-container',
      fullHeight ? 'ds-spinner-fullheight' : '',
      fullScreen ? 'ds-spinner-fullscreen' : '',
      className,
    ]
      .filter(Boolean)
      .join(' ');

    const spinnerClasses = ['ds-spinner', `ds-spinner-${size}`, text ? 'ds-spinner-with-text' : '']
      .filter(Boolean)
      .join(' ');

    return (
      <div ref={ref} className={containerClasses} role="status" aria-live="polite" {...props}>
        <div className={spinnerClasses} />
        {text && <p className="ds-spinner-text">{text}</p>}
        <span className="ds-visually-hidden">Loading...</span>
      </div>
    );
  }
);

Spinner.displayName = 'Spinner';
