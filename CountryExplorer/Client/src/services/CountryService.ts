import axios from 'axios';
import { CountrySummary, CountryDetail } from '../models/Country';

const API_BASE_URL = '/api'; // Change to align with BE API`

export const fetchAllCountries = async (): Promise<CountrySummary[]> => {
  const response = await axios.get<CountrySummary[]>(`${API_BASE_URL}/countries`);
  return response.data;
};

export const fetchCountryByName = async (name: string): Promise<CountryDetail> => {
  const response = await axios.get<CountryDetail>(`${API_BASE_URL}/countries/${name}`);
  return response.data;
};
