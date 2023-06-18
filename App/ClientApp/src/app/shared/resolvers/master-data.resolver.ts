import { HttpClient } from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, map, of } from 'rxjs';
import { CacheService } from '../services/cache.service';

export interface CountryDto {
  id: number;
  name: string;
  abbreviation: string;
  phonePrefix: string;
}

export interface CityDto {
  id: number;
  name: string;
  countryId: number;
}

export interface MasterDataDto {
  countries: CountryDto[];
  cities: CityDto[];
}

export const masterDataResolver = (
  http = inject(HttpClient),
  cache = inject(CacheService)
): Observable<MasterDataDto> => {
  if (cache.masterData) return of(cache.masterData);

  return http.get<MasterDataDto>('masterData').pipe(
    map((masterData) => {
      cache.masterData = masterData;
      return masterData;
    }),
    map((masterData) => masterData)
  );
};
