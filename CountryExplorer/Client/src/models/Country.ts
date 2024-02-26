export interface CountrySummary {
  name: CountryName;
  capital: string[];
  currencies: { [key: string]: Currency };
  languages: { [key: string]: string };
  region: string;
  subregion: string;
}

export interface CountryDetail extends CountrySummary {
  flags: Flag;
}

export interface Flag {
  png: string;
  alt: string;
}

export interface CountryName {
  common: string;
  official: string;
}

export interface Currency {
  name: string;
  symbol: string;
}