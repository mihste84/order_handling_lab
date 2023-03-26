export interface WaitActivityMetadata {
  name: string
  description: string
  input: WaitInputMetadata
}

export interface WaitInputMetadata {
  name: string
  type: string
  validators: WaitInputValidators
}

export interface WaitInputValidators {
  required: MetadataRequiredValidator
  maxLength: MetadataMaxLengthValidator
  minLength: MetadataMinLengthValidator
}

export interface MetadataMaxLengthValidator {
  message: string
  maxLength: number
}

export interface MetadataMinLengthValidator {
  message: string
  maxLength: number
}

export interface MetadataRequiredValidator {
  message: string
}
