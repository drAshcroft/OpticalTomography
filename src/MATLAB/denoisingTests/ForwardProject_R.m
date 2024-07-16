D=imread('untitled.png');
imagesc(D);
D=D(:,:,1);

Start = (272-248)/2;
D=D(Start:end-Start,:);
D=D(2:end,:);

nProjections = 100;

angleStep=360.0/nProjections;

Projections=cell([1 nProjections]);

thetas = ((1:nProjections)-1)*angleStep;

Projections = radon(D,thetas);


reconImage=iradon(Projections,thetas,'linear','Ram-Lak');

figure;
imagesc(D);
colormap gray;

figure;
imagesc(reconImage);
colormap gray;