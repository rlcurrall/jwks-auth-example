import { forwardRef, InputHTMLAttributes, ReactNode } from 'react';
import { FormGroup } from './form-group';
import './input.css';

export interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
  success?: string;
  helper?: string;
  fullWidth?: boolean;
  required?: boolean;
  iconLeft?: ReactNode;
  iconRight?: ReactNode;
  spacing?: 'sm' | 'md' | 'lg' | 'none';
  layout?: 'vertical' | 'horizontal';
  className?: string;
  wrapperClassName?: string;
  labelClassName?: string;
  fieldClassName?: string;
}

export const Input = forwardRef<HTMLInputElement, InputProps>(
  (
    {
      label,
      error,
      success,
      helper,
      fullWidth = true,
      required = false,
      iconLeft,
      iconRight,
      spacing,
      layout,
      className = '',
      wrapperClassName = '',
      labelClassName = '',
      fieldClassName = '',
      id,
      ...props
    },
    ref
  ) => {
    const inputClasses = [
      'ds-input',
      error ? 'ds-input-error' : '',
      success && !error ? 'ds-input-success' : '',
      fullWidth ? 'ds-input-full' : '',
      className,
    ]
      .filter(Boolean)
      .join(' ');

    // Determine variant based on status
    const variant = error ? 'error' : success ? 'success' : 'default';

    return (
      <FormGroup
        label={label}
        error={error}
        success={success}
        helper={helper}
        htmlFor={id}
        required={required}
        variant={variant}
        spacing={spacing}
        layout={layout}
        iconLeft={iconLeft}
        iconRight={iconRight}
        className={wrapperClassName}
        labelClassName={labelClassName}
        fieldClassName={fieldClassName}
      >
        <input
          ref={ref}
          id={id}
          className={inputClasses}
          aria-invalid={!!error}
          required={required}
          {...props}
        />
      </FormGroup>
    );
  }
);

Input.displayName = 'Input';
