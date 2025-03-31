import { HTMLAttributes, ReactNode, forwardRef } from 'react';
import './form-group.css';

export type FormGroupVariant = 'default' | 'error' | 'success';
export type FormGroupLayout = 'vertical' | 'horizontal';
export type FormGroupSpacing = 'sm' | 'md' | 'lg' | 'none';

export interface FormGroupProps extends HTMLAttributes<HTMLDivElement> {
  /**
   * Label for the form field
   */
  label?: string;

  /**
   * Error message to display
   */
  error?: string;

  /**
   * Success message to display
   */
  success?: string;

  /**
   * Helper text to display below the field
   */
  helper?: string;

  /**
   * The ID of the form control (for the htmlFor attribute)
   */
  htmlFor?: string;

  /**
   * The content of the form group (usually an input or other form control)
   */
  children: ReactNode;

  /**
   * Whether the field is required
   * @default false
   */
  required?: boolean;

  /**
   * Display variant
   * @default 'default'
   */
  variant?: FormGroupVariant;

  /**
   * Layout direction
   * @default 'vertical'
   */
  layout?: FormGroupLayout;

  /**
   * Bottom margin size
   * @default 'md'
   */
  spacing?: FormGroupSpacing;

  /**
   * Icon to display at the start of the input
   */
  iconLeft?: ReactNode;

  /**
   * Icon to display at the end of the input
   */
  iconRight?: ReactNode;

  /**
   * Additional CSS class for the form group
   */
  className?: string;

  /**
   * Additional CSS class for the label
   */
  labelClassName?: string;

  /**
   * Additional CSS class for the form field container
   */
  fieldClassName?: string;
}

/**
 * FormGroup component for grouping form controls with labels and feedback
 */
export const FormGroup = forwardRef<HTMLDivElement, FormGroupProps>(
  (
    {
      label,
      error,
      success,
      helper,
      htmlFor,
      children,
      required = false,
      variant = 'default',
      layout = 'vertical',
      spacing = 'md',
      iconLeft,
      iconRight,
      className = '',
      labelClassName = '',
      fieldClassName = '',
      ...props
    },
    ref
  ) => {
    // Determine variant based on error/success props if not explicitly set
    if (variant === 'default') {
      if (error) variant = 'error';
      else if (success) variant = 'success';
    }

    const formGroupClasses = [
      'ds-form-group',
      variant !== 'default' ? `ds-form-group-${variant}` : '',
      layout === 'horizontal' ? 'ds-form-group-horizontal' : '',
      spacing !== 'md'
        ? spacing === 'none'
          ? 'ds-form-group-no-margin'
          : `ds-form-spacing-${spacing}`
        : '',
      className,
    ]
      .filter(Boolean)
      .join(' ');

    const labelClasses = ['ds-form-label', required ? 'ds-form-label-required' : '', labelClassName]
      .filter(Boolean)
      .join(' ');

    const fieldClasses = [
      'ds-form-field',
      iconLeft ? 'ds-with-icon-left' : '',
      iconRight ? 'ds-with-icon-right' : '',
      fieldClassName,
    ]
      .filter(Boolean)
      .join(' ');

    return (
      <div ref={ref} className={formGroupClasses} {...props}>
        {label && (
          <label className={labelClasses} htmlFor={htmlFor}>
            {label}
          </label>
        )}

        <div className={fieldClasses}>
          {iconLeft && <span className="ds-form-icon ds-form-icon-left">{iconLeft}</span>}
          {children}
          {iconRight && <span className="ds-form-icon ds-form-icon-right">{iconRight}</span>}
        </div>

        {helper && !error && !success && <div className="ds-form-helper">{helper}</div>}
        {error && <div className="ds-form-error">{error}</div>}
        {success && !error && <div className="ds-form-success">{success}</div>}
      </div>
    );
  }
);

FormGroup.displayName = 'FormGroup';
