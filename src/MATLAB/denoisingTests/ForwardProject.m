D=imread('untitled.png');
imagesc(D);
D=D(:,:,1);

Start = (272-248)/2;
D=D(Start:end-Start,:);
D=D(2:end,:);

nProjections = 100;

angleStep=360.0/nProjections;

Projections=cell([1 nProjections]);
for I=1:nProjections
   tempImage = imrotate(D,-1*angleStep*I,'bilinear','crop'); 
    
   Projections{I}=sum(tempImage);   
end