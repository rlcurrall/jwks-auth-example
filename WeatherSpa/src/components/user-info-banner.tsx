import { HTMLAttributes, forwardRef } from 'react';
import { User } from '../types';
import './user-info-banner.css';

export type UserInfoBannerVariant = 'default' | 'primary';

export interface UserInfoBannerProps extends HTMLAttributes<HTMLDivElement> {
  /**
   * User data to display in the banner
   */
  user: User;

  /**
   * Visual style variant
   * @default 'default'
   */
  variant?: UserInfoBannerVariant;

  /**
   * Whether to use a more compact layout
   * @default false
   */
  compact?: boolean;

  /**
   * Scopes to display as badges
   * By default shows all scopes in user.scopes
   */
  displayedScopes?: string[];

  /**
   * Additional CSS class
   */
  className?: string;
}

/**
 * User information banner component
 * Displays user information and permission scopes
 */
export const UserInfoBanner = forwardRef<HTMLDivElement, UserInfoBannerProps>(
  (
    {
      user,
      variant = 'default',
      compact = false,
      displayedScopes = ['read', 'write'],
      className = '',
      ...props
    },
    ref
  ) => {
    const bannerClasses = [
      'ds-user-info-banner',
      variant !== 'default' ? variant : '',
      compact ? 'compact' : '',
      className,
    ]
      .filter(Boolean)
      .join(' ');

    return (
      <div ref={ref} className={bannerClasses} {...props}>
        <div className="ds-user-info-details">
          <span className="ds-user-info-username">{user.username}</span>

          {user.tenant && (
            <>
              <span className="ds-user-info-divider">|</span>
              <span className="ds-user-info-tenant">{user.tenant}</span>
            </>
          )}
        </div>

        <div className="ds-user-info-scopes">
          {displayedScopes.map(scope => {
            const isActive = user.scopes.includes(scope);

            return (
              <div
                key={scope}
                className={`ds-scope-badge ${isActive ? 'active' : 'inactive'}`}
                title={isActive ? `Has ${scope} permission` : `Does not have ${scope} permission`}
              >
                <span className="ds-scope-badge-icon">{isActive ? '✓' : '✗'}</span>
                {scope.charAt(0).toUpperCase() + scope.slice(1)}
              </div>
            );
          })}
        </div>
      </div>
    );
  }
);

UserInfoBanner.displayName = 'UserInfoBanner';
