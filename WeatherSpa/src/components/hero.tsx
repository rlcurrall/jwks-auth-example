import { HTMLAttributes, ReactNode, forwardRef } from 'react';
import './hero.css';
import { Heading } from './typography';

export interface HeroProps extends HTMLAttributes<HTMLElement> {
  /**
   * Hero title
   */
  title: string;

  /**
   * Optional subtitle
   */
  subtitle?: string;

  /**
   * Size variant
   * @default 'default'
   */
  size?: 'sm' | 'default' | 'lg';

  /**
   * Whether to show the rain animation
   * @default true
   */
  showAnimation?: boolean;

  /**
   * Hero content (below title and subtitle)
   */
  children?: ReactNode;

  /**
   * Additional CSS class
   */
  className?: string;
}

/**
 * Hero component for page headers with animations
 */
export const Hero = forwardRef<HTMLElement, HeroProps>(
  (
    { title, subtitle, size = 'default', showAnimation = true, children, className = '', ...props },
    ref
  ) => {
    const heroClasses = [
      'ds-hero',
      size === 'sm' ? 'ds-hero-sm' : '',
      size === 'lg' ? 'ds-hero-lg' : '',
      !showAnimation ? 'ds-hero-no-animation' : '',
      className,
    ]
      .filter(Boolean)
      .join(' ');

    return (
      <header ref={ref} className={heroClasses} {...props}>
        <div className="ds-hero-content">
          <Heading level={1} className="ds-hero-title">
            {title}
          </Heading>

          {subtitle && <p className="ds-hero-subtitle">{subtitle}</p>}

          {children}
        </div>
      </header>
    );
  }
);

Hero.displayName = 'Hero';
