import { forwardRef, HTMLAttributes, ReactNode } from 'react';
import './message.css';

export type MessageVariant = 'info' | 'success' | 'warning' | 'error';

export interface MessageProps extends HTMLAttributes<HTMLDivElement> {
  variant: MessageVariant;
  title?: string;
  showIcon?: boolean;
  className?: string;
  children: ReactNode;
}

export const Message = forwardRef<HTMLDivElement, MessageProps>(
  ({ variant = 'info', title, showIcon = true, className = '', children, ...props }, ref) => {
    const messageClasses = ['ds-message', `ds-message-${variant}`, className]
      .filter(Boolean)
      .join(' ');

    // Icons for different message types
    const getIcon = () => {
      switch (variant) {
        case 'success':
          return '✓';
        case 'error':
          return '✕';
        case 'warning':
          return '⚠';
        case 'info':
        default:
          return 'ℹ';
      }
    };

    return (
      <div ref={ref} className={messageClasses} role="alert" {...props}>
        {showIcon && (
          <span className="ds-message-icon" aria-hidden="true">
            {getIcon()}
          </span>
        )}
        <div className="ds-message-content">
          {title && <h4 className="ds-message-title">{title}</h4>}
          <div className="ds-message-body">{children}</div>
        </div>
      </div>
    );
  }
);

Message.displayName = 'Message';
