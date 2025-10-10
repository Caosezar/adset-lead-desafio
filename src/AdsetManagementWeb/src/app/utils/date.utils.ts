export function getYearsList(startYear: number = 2000, endYear?: number): number[] {
  const currentYear = endYear || new Date().getFullYear();
  const years: number[] = [];
  
  for (let year = currentYear; year >= startYear; year--) {
    years.push(year);
  }
  
  return years;
}

export const VEHICLE_YEARS = getYearsList(2000, 2024);


export type VehicleYear = number;
export type YearRange = {
  min?: VehicleYear;
  max?: VehicleYear;
};